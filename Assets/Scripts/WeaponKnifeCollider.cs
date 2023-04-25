using System.Collections;
using UnityEngine;

public class WeaponKnifeCollider : MonoBehaviour
{
    [SerializeField] private ImpactMemoryPool _impactMemoryPool;
    [SerializeField] private Transform _knifeTransform;

    private Collider _collider;
    private int _damage;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void StartCollider(int damage)
    {
        _damage = damage;
        _collider.enabled = true;

        StartCoroutine(nameof(DisableByTime), 0.1f);
    }

    private IEnumerator DisableByTime(float time)
    {
        yield return new WaitForSeconds(time);

        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _impactMemoryPool.SpawnImpact(other, _knifeTransform);

        if (other.CompareTag("ImpactEnemy"))
        {
            other.GetComponentInParent<EnemyFSM>().TakeDamage(_damage);
        }
        else if (other.CompareTag("InteractionObject"))
        {
            other.GetComponent<InteractionObject>().TakeDamage(_damage);
        }
    }
}
