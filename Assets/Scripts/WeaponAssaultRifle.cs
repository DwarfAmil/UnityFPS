using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _audioClipTakeOutWeapon;

    private AudioSource _audioSource;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // 무기 장착 사운드 재생
        PlaySound(_audioClipTakeOutWeapon);
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
