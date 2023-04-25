using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable] public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;

    [Header("HP")]
    [SerializeField] private int _maxHP = 100;
    private int _currentHP;

    public float walkSpeed => _walkSpeed;
    public float runSpeed => _runSpeed;

    public int currentHP => _currentHP;
    public int maxHP => _maxHP;

    private void Awake()
    {
        _currentHP = _maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = _currentHP;

        // 남은 채력에서 받은 데미지를 뺏을 때 0보다 작다면 0을 넣고, 아니라면 뺀 값을 넣어라
        _currentHP = _currentHP - damage > 0 ? _currentHP - damage : 0;
        
        onHPEvent.Invoke(previousHP, _currentHP);

        if (_currentHP == 0)
        {
            return true;
        }

        return false;
    }

    public void IncreaseHP(int hp)
    {
        int previousHP = _currentHP;

        _currentHP = _currentHP + hp > maxHP ? maxHP : _currentHP + hp;
        
        onHPEvent.Invoke(previousHP, currentHP);
    }
}
