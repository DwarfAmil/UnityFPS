using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRevolver : WeaponBase
{
    // 총구 이펙트 (on / off)
    [Header("Fire Effects")]
    [SerializeField] private GameObject _muzzleFlashEffect;

    // 총알 생성 위치
    [Header("Spawn Point")]
    [SerializeField] private Transform _bulletSpawnPoint;

    // 공격, 장전 사운드
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _audioClipFire;
    [SerializeField] private AudioClip _audioClipReload;

    // 공격 효과 생성 후 활성 / 비활성 관리
    private ImpactMemoryPool _impactMemoryPool;

    // 광선 발사
    private Camera _mainCamera;

    private void OnEnable()
    {
        // 총구 이펙트 오브젝트 비활성화
        _muzzleFlashEffect.SetActive(false);
        
        // 무기가 활성화 될 때 해당 무기의 탄창 정보를 갱신
        onMagazineEvent.Invoke(_weaponSetting.currentMagazine);
        
        // 무기가 활성화 될 때 해당 무기의 탄 수 정보를 갱신
        onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);

        ResetVariables();
    }

    private void Awake()
    {
        base.Setup();

        _impactMemoryPool = GetComponent<ImpactMemoryPool>();
        _mainCamera = Camera.main;

        // 처음 탄창 수 는 최대로 설정
        _weaponSetting.currentMagazine = _weaponSetting.maxMagazine;

        // 처음 탄 수는 최대로 설정
        _weaponSetting.currentAmmo = _weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && _isAttack == false && _isReload == false)
        {
            OnAttack();
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        _isAttack = false;
    }

    public override void StartReload()
    {
        // 현재 재장전 중이거나 탄창 수가 0이면 재장전 불가
        if (_isReload == true || _weaponSetting.currentMagazine <= 0) return;
        
        // 무기 액션 도중에 'R' 키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine(nameof(OnReload));
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

            // 무기 애니메이션 재생
            _animator.Play("Fire", -1, 0);
            
            // 총구 이팩트 재생
            StartCoroutine("OnMuzzleFlashEffect");

            // 공격 사운드 재생
            PlaySound(_audioClipFire);
            
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

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(_weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(_weaponSetting.damage);
            }
        }
        
        Debug.DrawRay(_bulletSpawnPoint.position, attackDirection * _weaponSetting.attackDistance, Color.blue);
    }
    
    private void ResetVariables()
    {
        _isReload = false;
        _isAttack = false;
    }
}
