using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed = 4;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(OnFadeEffect));
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(OnFadeEffect));
    }

    private IEnumerator OnFadeEffect()
    {
        while (true)
        {
            Color color = _meshRenderer.material.color;
            // float f = Mathf.PingPong(float t, float length);
            // t 값에 따라 0부터 length 사이의 값이 반환
            // t 값이 계속 증가할 때 length까지는 t 값을 반환하고, t가 length보다 커졌을 때 순차적으로 0까지 -, length까지 +를 반복
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.deltaTime * _fadeSpeed, 1));
            _meshRenderer.material.color = color;

            yield return null;
        }
    }
}
