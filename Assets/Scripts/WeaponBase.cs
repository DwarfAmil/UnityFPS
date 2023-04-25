using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Main, Sub, Melee, Throw }

[Serializable] public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

[Serializable] public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("WeaponBase")]
    // 무기 종류
    [SerializeField] protected WeaponType _weaponType;
    
    // 무기 설정
    [SerializeField] protected WeaponSetting _weaponSetting;

    // 마지막 발사시간 체크용
    protected float _lastAttackTime = 0;
    
    // 재장전 중인지 체크
    protected bool _isReload = false;
    
    // 공격 여부 체크용
    protected bool _isAttack = false;
    
    // 사운드 재생 컴포넌트
    protected AudioSource _audioSource;
    
    // 애니메이션 재생 제어
    protected PlayerAnimatorController _animator;
    
    // 외부에서 이벤트 함수 등록을 할 수 있도록 public 선언
    [HideInInspector] public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector] public MagazineEvent onMagazineEvent = new MagazineEvent();

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public PlayerAnimatorController animator => _animator;
    public WeaponName WeaponName => _weaponSetting.weaponName;
    public int CurrentMagazine => _weaponSetting.currentMagazine;
    public int MaxMagazine => _weaponSetting.maxMagazine;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        // 기존 재생중인 사운드 정지, 새로운 사운드 clip으로 교체 후 사운드 재생
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    protected void Setup()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<PlayerAnimatorController>();
    }
}
