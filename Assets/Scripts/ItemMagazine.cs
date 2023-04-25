using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemMagazine : ItemBase
{
    [SerializeField] private GameObject _magazineEffectPrefab;
    [SerializeField] private int _increaseMazine = 2;
    [SerializeField] private float _moveDis = 0.2f;
    [SerializeField] private float _pingpongSpeed = 0.5f;
    [SerializeField] private float _rotateSpeed = 50;

    private IEnumerator Start()
    {
        float y = transform.position.y;
        
        while (true)
        {
            // y축을 기준으로 회전
            transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
            
            // 처음 배치된 위치를 기준으로 y 위치를 위, 아래로 이동
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(y, y + _moveDis, Mathf.PingPong(Time.time * _pingpongSpeed, 1));
            transform.position = pos;

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        entity.GetComponentInChildren<WeaponAssaultRifle>().IncreaseMagazine(_increaseMazine);

        Instantiate(_magazineEffectPrefab, transform.position, quaternion.identity);
        
        Destroy(gameObject);
    }
}
