using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroyerByTime : MonoBehaviour
{
    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // 파티클이 재생중이 아니면 삭제
        if (_particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
