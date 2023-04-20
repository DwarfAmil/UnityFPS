using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CasingMemoryPool : MonoBehaviour
{
    // 탄피 오브젝트
    [SerializeField] private GameObject _casingPrefab;

    // 탄피 메모리풀
    private MemoryPool _memoryPool;

    private void Awake()
    {
        _memoryPool = new MemoryPool(_casingPrefab);
    }

    public void SpawnCasing(Vector3 pos, Vector3 dir)
    {
        GameObject item = _memoryPool.ActivatePoolItem();
        item.transform.position = pos;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(_memoryPool, dir);
    }
}
