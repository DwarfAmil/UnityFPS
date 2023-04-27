using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemGrenade : ItemBase
{
    [SerializeField] private GameObject _magazineEffectPrefab;
    [SerializeField] private int _increaseAmmo;
    [SerializeField] private int _increaseMazine = 2;
    
    public override void Use(GameObject entity)
    {
        
    }

    public override void UseAmmo(GameObject entity)
    {
        entity.GetComponentInChildren<WeaponSwitchSystem>().IncreaseAmmo(_increaseAmmo);
        
        Instantiate(_magazineEffectPrefab, transform.position, quaternion.identity);
        
        Destroy(gameObject);
    }
}
