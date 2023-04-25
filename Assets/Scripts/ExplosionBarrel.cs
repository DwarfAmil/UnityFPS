using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")]
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _explosionDelayTime = 0.3f;
    [SerializeField] private float _explosionRadius = 10.0f;
    [SerializeField] private float _explosionForce = 1000.0f;

    private bool _isExplode = false;

    public override void TakeDamage(int damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0 && _isExplode == false)
        {
            StartCoroutine(nameof(ExplodeBarrel));
        }
    }

    private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(_explosionDelayTime);

        _isExplode = true;

        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(
            _explosionPrefab,
            new Vector3(bounds.center.x, bounds.min.y, bounds.center.z),
            transform.rotation
            );

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider hit in colliders)
        {
            // 부딪힌 오브젝트가 플레이어면 처리
            PlayerController player = hit.GetComponent<PlayerController>();
            
            if (player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // 부딪힌 오브젝트가 적 캐릭터일 대 처리
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }
            
            // 부딪힌 오브젝트가 상호작용 오브젝트면 처리
            InteractionObject interaction = hit.GetComponent<InteractionObject>();

            if (interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // 중력을 가지고 있는 오브젝트면 밀려나도록 처리
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }
        
        // Barrel삭제
        Destroy(gameObject);
    }
}
