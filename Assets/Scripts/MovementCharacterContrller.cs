using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterContrller : MonoBehaviour
{
    // 이동 속도
    [SerializeField] private float moveSpeed;
    
    //이동 힘 (X, Z와 Y축을 별도로 계산해서 실제 이동에 적용)
    private Vector3 moveForce;

    // 점프 힘
    [SerializeField] private float jumpForce;

    //중력 계수
    [SerializeField] private float gravity;
    
    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }

    //플레이어 이동 제어를 위한 컴포넌트
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 허공에 떠있으면 중력만큼 Y축 이동 속도 감소
        if (!_characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }
        
        // 1초당 moveForce의 속력으로 이동
        _characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        // 이동 방향 = 캐릭터의 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);
        
        // 이동 힘 = 이동 방향 * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        // 플레이어가 바닥에 있을 때만 점프 가능
        if (_characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
}
