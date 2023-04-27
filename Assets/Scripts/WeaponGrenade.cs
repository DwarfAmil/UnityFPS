using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponGrenade : WeaponBase
{
    // 공격 사운드
    [Header("Audio Clip")] 
    [SerializeField] private AudioClip _audioClipFire;

    // 수류탄 프리팹, 생성 위치
    [Header("Grenade")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private Transform _grenadeSpawnPoint;

    private void OnEnable()
    {
        // 무기가 활성화 될 때 해당 무기의 탄창 정보 갱신
        onMagazineEvent.Invoke(_weaponSetting.currentMagazine);
        
        // 무기가 활성화 될 때 해당 무기의 탄 수 정보 갱신
        onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);
    }

    private void Awake()
    {
        base.Setup();

        // 처음 탄창 수는 최대로
        _weaponSetting.currentMagazine = _weaponSetting.maxMagazine;

        // 처음 탄 수는 최대로
        _weaponSetting.currentAmmo = _weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && _isAttack == false && _weaponSetting.currentAmmo > 0)
        {
            StartCoroutine(nameof(OnAttack));
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        _isAttack = false;
    }

    public override void StartReload()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator OnAttack()
    {
        _isAttack = true;
        
        // 공격 애니메이션 재생
        _animator.Play("Fire", -1, 0);
        
        // 공격 사운드 재생
        PlaySound(_audioClipFire);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (_animator.CurrentAnimationIs("Movement"))
            {
                _isAttack = false;
                
                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// arms_assult_rifle_01.fbx의
    /// grenade_throw@assult_rifle_01 애니메이션 이벤트 함수
    /// </summary>
    public void SpawnGrenadeProjectile()
    {
        GameObject grenadeClone = Instantiate(_grenadePrefab, _grenadeSpawnPoint.position, Random.rotation);
        grenadeClone.GetComponent<WeaponGrenadeProjectile>().Setup(_weaponSetting.damage, transform.parent.forward);

        _weaponSetting.currentAmmo--;
        onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);
    }
    
    public void IncreaseAmmo(int ammo)
    {
        // 수류탄은 탄창이 없고, 탄 수를 수류탄 개수로 사용하기 때문에 탄 수를 증가시킴
        _weaponSetting.currentAmmo =
            _weaponSetting.currentAmmo + ammo > _weaponSetting.maxAmmo ? _weaponSetting.maxAmmo : _weaponSetting.currentAmmo + ammo;
        
        onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);
    }
}
