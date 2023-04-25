using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")]
    [SerializeField] private GameObject _destructibleBarrelPieces;

    private bool _isDestroyed = false;

    public override void TakeDamage(int damage)
    {
        _currentHP -= damage;

        if (_currentHP < 0 && _isDestroyed == false)
        {
            _isDestroyed = true;

            Instantiate(_destructibleBarrelPieces, transform.position, transform.rotation);
            
            Destroy(gameObject);
        }
    }
}
