using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementTransform : MonoBehaviour
{
    [SerializeField] private float _speed = 0.0f;

    [SerializeField] private Vector3 _dir = Vector3.zero;

    /// <summary>
    /// 이동 방향이 설정되면 알아서 이동하도록 함
    /// </summary>
    private void Update()
    {
        transform.position += _dir * _speed * Time.deltaTime;
    }

    /// <summary>
    /// 외부에서 매개변수로 이동 방향을 설정
    /// </summary>
    public void MoveTo(Vector3 dir)
    {
        _dir = dir;
    }
}
