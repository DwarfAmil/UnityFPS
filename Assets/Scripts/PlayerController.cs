using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    // 달리기 키 세팅
    [SerializeField] private KeyCode _keyCodeRun = KeyCode.LeftShift;
    
    // 점프 키 세팅
    [SerializeField] private KeyCode _keyCodeJump = KeyCode.Space;

    [Header("Audio Clips")]
    // 걷기 사운드 클립
    [SerializeField] private AudioClip _audioClipWalk;

    // 달리기 사운드 클립
    [SerializeField] private AudioClip _audioClipRun;
    
    // 마우스 이동으로 카메라 회전
    private RotateToMouse _rotateToMouse;
    
    // 키보드 입력으로 플레이어 이동, 점프
    private MovementCharacterContrller _movement;
    
    // 이동 속도 등의 플레이어 정보
    private Status _status;
    
    // 애니메이션 재생 제어
    private PlayerAnimatorController _animator;

    // 사운드 재생 제어
    private AudioSource _audioSource;

    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정
        Cursor.visible = false;
        
        // 마우스를 현재 위치에 고정시킴
        Cursor.lockState = CursorLockMode.Locked;

        _rotateToMouse = GetComponent<RotateToMouse>();
        _movement = GetComponent<MovementCharacterContrller>();
        _status = GetComponent<Status>();
        _animator = GetComponent<PlayerAnimatorController>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        _rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        // 이동중 일 때 (걷기 or 달리기)
        if (x != 0 || z != 0)
        {
            bool isRun = false;
            
            // 옆이나 뒤로 이동할 때는 달릴 수 없다
            if (z > 0)
                isRun = Input.GetKey(_keyCodeRun);

            // 삼항연산자 이용 (조건 ? true : false)
            // isRun이 true면 status.RunSpeed를 실행, false면 status.WalkSpeed를 실행
            _movement.MoveSpeed = isRun == true ? _status.RunSpeed : _status.WalkSpeed;
            
            // isRun이 true면 Animator에 있는 "movementSpeed" 파라미터에 1을 넣음, false면 0.5를 넣음
            _animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            
            // isRun이 true이면 달리기 사운드 재생, false이면 걷기 사운드 재생
            _audioSource.clip = isRun == true ? _audioClipRun : _audioClipWalk;

            // 방향키 입력 여부는 매 프레임 확인하기에 재생중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
            if (_audioSource.isPlaying == false)
            {
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }
        
        // 제자리에 멈춰있을 때
        else
        {
            _movement.MoveSpeed = 0;
            _animator.MoveSpeed = 0;

            // 멈췄을 때 사운드가 재생중이면 정지
            if (_audioSource.isPlaying == true)
            {
                _audioSource.Stop();
            }
        }
        
        _movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(_keyCodeJump))
        {
            _movement.Jump();
        }
    }
}
