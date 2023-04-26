using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGrenadeProjectile : MonoBehaviour
{
    [Header("Explosion Barrel")]
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _explosionRadius = 10.0f;
    [SerializeField] private float _explosionForce = 500.0f;
    [SerializeField] private float _throwForce = 1000.0f;

    private int _explosionDamage;
    private Rigidbody _rigidbody;

    public void Setup(int damage, Vector3 rot)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(rot * _throwForce);

        _explosionDamage = damage;
    }

    private void OnCollisionEnter(Collision other)
    {
        // 폭팔 이펙트 생성
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        
        // 폭팔 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭팔 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider hit in colliders)
        {
            // 폭팔 범위에 부딪힌 오브젝트가 플레이어이면 처리
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage((int)(_explosionDamage * 0.2f));
                continue;
            }

            // 폭팔 범위에 부딪힌 오브젝트가 적 캐릭터이면 처리
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(_explosionDamage);
                continue;
            }

            // 폭팔 범위에 부딪힌 오브젝트가 상호작용 오브젝트이면 TakeDamage()로 피해를 줌
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                interaction.TakeDamage(_explosionDamage);
            }

            // 중력을 가진 오브젝트면 밀려나도록
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }
        
        // 수류탄 오브젝트 삭제
        Destroy(gameObject);
    }
}