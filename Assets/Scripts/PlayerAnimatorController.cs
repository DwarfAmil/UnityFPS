using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        // "Player" 오브젝트의 기준으로 자식 오브젝트인 "arm_assualt_rifle_01" 오브젝트에 Animator 컴포넌트가 있다.
        _animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        // Animator View에 있는 float 타입 변수 "ParamName"의 값을 value로 설정
        set => _animator.SetFloat("movementSpeed", value);
        
        // Animator View에 있는 float 타입 변수 "ParamName"의 값을 반환
        get => _animator.GetFloat("movementSpeed");
    }

    // _animator.Play()를 외부에서 사용 할 수 있도록 정의
    public void Play(string stateName, int layer, float normalizedTime)
    {
        _animator.Play(stateName, layer, normalizedTime);
    }

}
