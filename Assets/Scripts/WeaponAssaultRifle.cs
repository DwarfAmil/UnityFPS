using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Fire Effects")]
    // 총구 이팩트 (On / Off)
    [SerializeField] private GameObject _muzzleFlashEffect;
    
    [Header("Audio Clips")]
    // 무기 장착 사운드
    [SerializeField] private AudioClip _audioClipTakeOutWeapon;
    
    // 공격 사운드
    [SerializeField] private AudioClip _audioClipFire;

    [Header("Weapon Setting")]
    // 무기 설정
    [SerializeField] private WeaponSetting _weaponSetting; 
    
    // 마지막 발사 시간 체크
    private float lastAttackTime = 0;

    // 사운드 재생 컴포넌트
    private AudioSource _audioSource;
    
    // 애니메이션 재생 제어
    private PlayerAnimatorController _animator;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInParent<PlayerAnimatorController>();
    }

    private void OnEnable()
    {
        // 무기 장착 사운드 재생
        PlaySound(_audioClipTakeOutWeapon);
        
        // 총구 이팩트 오브젝트 비활성화
        _muzzleFlashEffect.SetActive(false);
    }

    public void StartWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if (_weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
        
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 종료)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
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
        if (Time.time - lastAttackTime > _weaponSetting.attackRate)
        {
            // 뛰고 있을 때는 공격할 수 없다.
            if (_animator.MoveSpeed > 0.5f)
            {
                return;
            }
            
            // 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;
            
            // 무기 애니메이션 재생
            _animator.Play("Fire", -1, 0);
            
            // 총구 이팩트 재생
            StartCoroutine("OnMuzzleFlashEffect");
            
            // 공격 사운드 재생
            PlaySound(_audioClipFire);
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
}
