using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Casing : MonoBehaviour
{
    // 탄피 등장 후 비활성화 되는 시간
    [SerializeField] private float _deactiveTime = 5.0f;

    // 탄피가 회전하는 속력 계수
    [SerializeField] private float casingSpin = 1.0f;

    // 탄피가 부딪혔을 때 재생되는 사운드
    [SerializeField] private AudioClip[] _audioClips;

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private MemoryPool _memoryPool;

    public void Setup(MemoryPool pool, Vector3 dir)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _memoryPool = pool;
        
        // 탄피의 이동 속력과 회전 속력 설정
        _rigidbody.velocity = new Vector3(dir.x, 1.0f, dir.z);
        _rigidbody.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin), Random.Range(-casingSpin, casingSpin), Random.Range(-casingSpin, casingSpin));
        
        // 탄피 자동 비활성화를 위한 코루틴 실행
        StartCoroutine("DeactivateAfterTime");
    }

    private void OnCollisionEnter(Collision other)
    {
        // 여러 개의 탄피 사운드 중 임의의 사운드 선택
        int index = Random.Range(0, _audioClips.Length);
        _audioSource.clip = _audioClips[index];
        _audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(_deactiveTime);
        
        _memoryPool.DeactivePoolItem(this.gameObject);
    }
}
