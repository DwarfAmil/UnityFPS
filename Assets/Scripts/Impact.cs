using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem _particle;
    private MemoryPool _memoryPool;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    public void Setup(MemoryPool pool)
    {
        _memoryPool = pool;
    }

    private void Update()
    {
        // 파티클이 재생중이 아니면 삭제
        if (_particle.isPlaying == false)
        {
            _memoryPool.DeactivePoolItem(gameObject);
        }
    }
}
