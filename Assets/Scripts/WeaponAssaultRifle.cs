using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

[System.Serializable] public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector] public AmmoEvent onAmmoEvent = new AmmoEvent();

    [HideInInspector] public MagazineEvent onMagazineEvent = new MagazineEvent();
    
    [Header("Fire Effects")]
    // 총구 이팩트 (On / Off)
    [SerializeField] private GameObject _muzzleFlashEffect;

    [Header("Spawn Points")]
    // 탄피 생성 위치
    [SerializeField] private Transform _casingSpawnPoint;
    
    // 총알 생성 위치
    [SerializeField] private Transform _bulletSpawnPoint;
    
    [Header("Audio Clips")]
    // 무기 장착 사운드
    [SerializeField] private AudioClip _audioClipTakeOutWeapon;
    
    // 공격 사운드
    [SerializeField] private AudioClip _audioClipFire;
    
    // 재장전 사운드
    [SerializeField] private AudioClip _audioClipReload;

    [Header("Weapon Setting")]
    // 무기 설정
    [SerializeField] private WeaponSetting _weaponSetting;

    [Header("Aim UI")]
    // default / aim 모드에 따라 Aim 이미지 활성 / 비활성
    [SerializeField] private Image _imageAim;
    
    // 마지막 발사 시간 체크
    private float _lastAttackTime = 0;
    
    // 재장전 중인지 체크
    private bool _isReload = false;
    
    // 공격 여부 체크용
    private bool _isAttack = false;
    
    // 모드 전환 여부 체크용
    private bool _isModeChange;

    // 기본 모드에서의 카메라 FOV
    private float _defaultModeFOV = 60;

    // Aim 모드에서의 카메라 FOV
    private float _aimModeFOV = 30;

    // 사운드 재생 컴포넌트
    private AudioSource _audioSource;
    
    // 애니메이션 재생 제어
    private PlayerAnimatorController _animator;
    
    // 탄피 생성 후 활성 / 비활성 관리
    public CasingMemoryPool _casingMemoryPool;
    
    // 공격 효과 생성 후 활성 / 비활성 관리
    private ImpactMemoryPool _impactMemoryPool;
    
    // 광선 발사
    private Camera _mainCamera;
    
    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Poperty's
    public WeaponName WeaponName => _weaponSetting.weaponName;
    public int CurrentMagazine => _weaponSetting.currentMagazine;
    public int MaxMagazine => _weaponSetting.maxMagazine;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInParent<PlayerAnimatorController>();
        _casingMemoryPool = GetComponent<CasingMemoryPool>();
        _impactMemoryPool = GetComponent<ImpactMemoryPool>();
        _mainCamera = Camera.main;
        
        // 처음 탄창 수는 최대로 설정
        _weaponSetting.currentMagazine = _weaponSetting.maxMagazine;
        
        // 처음 탄약 수는 퇴대로 설정
        _weaponSetting.currentAmmo = _weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // 무기 장착 사운드 재생
        PlaySound(_audioClipTakeOutWeapon);
        
        // 총구 이팩트 오브젝트 비활성화
        _muzzleFlashEffect.SetActive(false);
        
        // 무기가 활성화 될 때 해당 무기의 탄창 정보를 갱신한다
        onMagazineEvent.Invoke(_weaponSetting.currentMagazine);
        
        // 무기가 활성화 될 때 해당 무기의 탄 수 정보를 갱신
        onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);

        ResetVariables();
    }

    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다
        if (_isReload == true) return;
        
        // 모드 전환중이면 무기 액션을 할 수 없다
        if (_isModeChange == true) return;

        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if (_weaponSetting.isAutomaticAttack == true)
            {
                _isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
        
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
        
        // 마우스 오른쪽 클릭 (모드 전환)
        else
        {
            // 공격 중일 때는 모드 전환을 할 수 없다
            if (_isAttack == true) return;

            StartCoroutine("OnModeChange");
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 종료)
        if (type == 0)
        {
            _isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        // 현재 재장전 중이면 재장전 불가능
        if (_isReload == true || _weaponSetting.currentMagazine <= 0) return;
        
        // 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if (Time.time - _lastAttackTime > _weaponSetting.attackRate)
        {
            // 뛰고 있을 때는 공격할 수 없다.
            if (_animator.MoveSpeed > 0.5f)
            {
                return;
            }
            
            // 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            _lastAttackTime = Time.time;
            
            // 탄약 수가 없으면 공격 불가능
            if (_weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            
            // 공격시 curruntAmmo를 1 감소, 탄 수 UI 업데이트
            _weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);

            // 무기 애니메이션 재생 (모드에 따라 AimFire of Fire 애니메이션 재생)
            // _animator.Play("Fire", -1, 0);
            string animation = _animator.AimModeIs == true ? "AimFire" : "Fire";
            _animator.Play(animation, -1, 0);
            
            // 총구 이팩트 재생 (default mode 일 때만 재생)
            if (_animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            // 공격 사운드 재생
            PlaySound(_audioClipFire);
            
            // 탄피 생성
            _casingMemoryPool.SpawnCasing(_casingSpawnPoint.position, transform.right);
            
            // 광선을 발사해 원하는 위치 공격 (+Impact Effect)
            TwoStepRaycast();
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        _muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(_weaponSetting.attackRate * 0.3f);
        
        _muzzleFlashEffect.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        // 기존에 재생중인 사운드 정지
        _audioSource.Stop();
        
        // 새로운 사운드 클립으로 교체
        _audioSource.clip = clip;
        
        // 교체된 사운드 재생
        _audioSource.Play();
    }

    private IEnumerator OnReload()
    {
        _isReload = true;
        
        // 재장전 애니메이션, 사운드 재생
        _animator.OnReload();
        PlaySound(_audioClipReload);

        while (true)
        {
            // 사운드가 재생중이 아니고, 현재 에니메이션이 Movement이면 재징전 애니메이션(, 사운드) 재생이 종료되었다는 뜻
            if (_audioSource.isPlaying == false && _animator.CurrentAnimationIs("Movement"))
            {
                _isReload = false;
                
                // 현재 탄창 수를 1 감소시키고, 바뀐 탄창 정보를 Text UI에 업데이트
                _weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(_weaponSetting.currentMagazine);
                
                // 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
                _weaponSetting.currentAmmo = _weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);
                
                yield break;
            }

            yield return null;
        }
    }

    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        
        // 화면 중앙의 좌표 (Aim 기준으로 Raycast연산)
        ray = _mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // 공격 사거리(AttackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if (Physics.Raycast(ray, out hit, _weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }

        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * _weaponSetting.attackDistance;
        }
        
        Debug.DrawRay(ray.origin, ray.direction * _weaponSetting.attackDistance, Color.red);
        
        //첫번째 Raycast연산으로 얻어진 targetPoint를 목표지점으로 설정하고, 총구를 시작 지점으로 하여 Raycast 연산
        Vector3 attackDirection = (targetPoint - _bulletSpawnPoint.position).normalized;

        if (Physics.Raycast(_bulletSpawnPoint.position, attackDirection, out hit, _weaponSetting.attackDistance))
        {
            _impactMemoryPool.SpawnImpact(hit);
        }
        
        Debug.DrawRay(_bulletSpawnPoint.position, attackDirection * _weaponSetting.attackDistance, Color.blue);
    }

    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0;

        _animator.AimModeIs = !_animator.AimModeIs;
        _imageAim.enabled = !_imageAim.enabled;

        float start = _mainCamera.fieldOfView;
        float end = _animator.AimModeIs == true ? _aimModeFOV : _defaultModeFOV;

        _isModeChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            
            // mode에 따라 카메라의 시야각을 변경
            _mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        _isModeChange = false;
    }

    private void ResetVariables()
    {
        _isReload = false;
        _isAttack = false;
        _isModeChange = false;
    }
    
}
