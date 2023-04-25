using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField] private WeaponKnifeCollider _weaponKnifeCollider;

    private void OnEnable()
    {
        _isAttack = false;
        
        // 무기가 활성화 될 때 해당 무기의 탄창 정보를 갱신
        onMagazineEvent.Invoke(_weaponSetting.currentMagazine);
        
        // 무기가 활성화 될 때 해당 무기의 탄 수 정보를 갱신
        onAmmoEvent.Invoke(_weaponSetting.currentAmmo, _weaponSetting.maxAmmo);
    }

    private void Awake()
    {
        base.Setup();

        // 처음 탄창 수 최대
        _weaponSetting.currentMagazine = _weaponSetting.maxMagazine;

        // 처음 탄 수 최대
        _weaponSetting.currentAmmo = _weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (_isAttack == true) return;

        // 연속 공격
        if (_weaponSetting.isAutomaticAttack == true)
        {
            StartCoroutine(nameof(OnAttackLoop), type);
        }
        
        // 단일 공격
        else
        {
            StartCoroutine(nameof(OnAttack), type);
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        _isAttack = false;
        StopCoroutine(nameof(OnAttackLoop));
    }

    public override void StartReload()
    {
        throw new NotImplementedException();
    }

    private IEnumerator OnAttackLoop(int type)
    {
        while (true)
        {
            yield return StartCoroutine(nameof(OnAttack), type);
        }
    }

    private IEnumerator OnAttack(int type)
    {
        _isAttack = true;
        
        // 공격 모션 선택 (0, 1)
        _animator.SetFloat("attackType", type);
        
        // 공격 애니메이션 재생
        _animator.Play("Fire", -1, 0);

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
    /// knife_attack_1@assult_rifle_01, knife_attack_1@assult_rifle_02
    /// 애니메이션 이벤트 함수
    /// </summary>
    public void StartWeaponKnifeCollider()
    {
        _weaponKnifeCollider.StartCollider(_weaponSetting.damage);
    }
}
