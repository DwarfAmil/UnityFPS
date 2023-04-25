using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ImpactType {Normal = 0, Obstacle, Enemy, InteractionObject, }

public class ImpactMemoryPool : MonoBehaviour
{
    // 피격 이펙트
    [SerializeField] private GameObject[] _impactPrefab;
    
    // 피격 이펙트 메모리풀
    private MemoryPool[] _memoryPool;

    private void Awake()
    {
        //피격 이펙트가 여러 종류이면 종류별로 _memoryPool 생성
        _memoryPool = new MemoryPool[_impactPrefab.Length];

        for (int i = 0; i < _impactPrefab.Length; i++)
        {
            _memoryPool[i] = new MemoryPool(_impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // 부딪힌 오브젝트의 Tag 정보에 따라 다르게 처리
        if (hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("InteractionObject"))
        {
            Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 pos, Quaternion rot, Color color = new Color())
    {
        GameObject item = _memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = pos;
        item.transform.rotation = rot;
        item.GetComponent<Impact>().Setup(_memoryPool[(int)type]);

        if (type == ImpactType.InteractionObject)
        {
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}
