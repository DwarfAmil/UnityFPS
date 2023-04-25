using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform _movement;
    
    // 발사체 최대 발사 거리
    private float _projectileDis = 30;

    // 발사체 공격력
    private int _damage = 5;

    public void Setup(Vector3 pos)
    {
        _movement = GetComponent<MovementTransform>();

        StartCoroutine(nameof(OnMove), pos);
    }

    private IEnumerator OnMove(Vector3 targetPos)
    {
        Vector3 start = transform.position;
        
        _movement.MoveTo((targetPos - transform.position).normalized);

        while (true)
        {
            if (Vector3.Distance(transform.position, start) >= _projectileDis)
            {
                Destroy(gameObject);
                
                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player Hit");
            other.GetComponent<PlayerController>().TakeDamage(_damage);
            
            Destroy(gameObject);
        }
    }
}
