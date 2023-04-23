using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyMemoryPool : MonoBehaviour
{
    // 적의 목표
    [SerializeField] private Transform _target;
    
    // 적이 등장하기 전 적의 등장 위치를 알려주기 위한 프리팹
    [SerializeField] private GameObject _enemySpawnPointPrefab;

    // 생성되는 적 프리팹
    [SerializeField] private GameObject _enemyPrefab;

    // 적 생성 주기
    [SerializeField] private float _enemySpawnTime = 1;

    // SpawnPoint 타일 생성 후 적이 등장하기까지의 대기 시간
    [SerializeField] private float _enemySpawnLatency = 1;

    // 적의 등장 위치를 알려주는 오브젝트 생성, 활성 / 비활성 관리
    private MemoryPool _spawnPointMemoryPool;
    
    // 적 생성, 활성 / 비활성 관리
    private MemoryPool _enemyMemoryPool;

    // 동시에 생성되는 적의 수
    private int _numberOfEnemiesSpawnedAtOnce = 1;

    // 맵 크기
    private Vector2Int _mapSize = new Vector2Int(100, 100);

    private void Awake()
    {
        _spawnPointMemoryPool = new MemoryPool(_enemySpawnPointPrefab);
        _enemyMemoryPool = new MemoryPool(_enemyPrefab);

        StartCoroutine(nameof(SpawnTile));
    }

    private IEnumerator SpawnTile()
    {
        var currentNumber = 0;
        const int maximumNumber = 50;

        while (true)
        {
            var item = _spawnPointMemoryPool.ActivatePoolItem();
            
            for (var i = 0; i < _numberOfEnemiesSpawnedAtOnce; i++)
            {
                item.transform.position = new Vector3(
                    Random.Range(-_mapSize.x * 0.49f, _mapSize.x * 0.49f),
                    1,
                    Random.Range(-_mapSize.y * 0.49f, _mapSize.y * 0.49f)
                );

                StartCoroutine(nameof(SpawnEnemy), item);
            }
            
            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                _numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(_enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(_enemySpawnLatency);

        // 적 생성, 적을 point 위치를 설정
        var item = _enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;
        
        item.GetComponent<EnemyFSM>().Setup(_target);
        
        // 타일 오브젝트를 비활성화
        _spawnPointMemoryPool.DeactivePoolItem(point);
    }
}
