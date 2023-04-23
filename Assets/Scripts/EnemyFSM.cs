using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public enum EnemyState {None = -1, Idle = 0, Wander, Pursuit, }

public class EnemyFSM : MonoBehaviour
{
    [Header("pursuit")]
    // 인식 범위 (이 범위 안에 들어오면 "Pursuit" 상태로 변경)
    [SerializeField] private float _targetRecognitionRange = 8;

    // 추적 범위 (이 범위 바깥으로 나가면 "Wander" 상태로 변경)
    [SerializeField] private float _pursuitLimitRange = 10;
    
    // 현재 적 행동
    private EnemyState _enemyState = EnemyState.None;

    // 이동 속도 등의 정보
    private Status _status;

    // 이동 제어를 위한 NavMeshAgent
    private NavMeshAgent _navMeshAgent;

    // 적의 공격 대상
    private Transform _target;

    //private void Awake()
    public void Setup(Transform target)
    {
        _status = GetComponent<Status>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        this._target = target;
        
        // NavMeshAgent 컴포넌트에서 회전을 업데이트 하지 않도록 설정
        _navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        // 적이 활성화 될 때 적의 상태를 "Idle"로 설정
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        // 적이 비활성화 될 때 현재 재생중인 상태를 종료하고, 상태를 "None"으로 설정
        StopCoroutine(_enemyState.ToString());

        _enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 Return
        if (_enemyState == newState) return;
        
        // 이전에 재생중이던 상태 종료
        StopCoroutine(_enemyState.ToString());

        // 현재 적의 상태를 newState로 설정
        _enemyState = newState;

        // 새로운 상태 재생
         StartCoroutine(_enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n초 후 "Wander" 상태로 변경하는 코루틴 실행
        StartCoroutine(nameof(AutoChangeFromIdleToWander));

        while (true)
        {
            // 대기 상태시 행동
            // 타겟과의 거리에 따라 행동 선택
            CalculateDistanceToTargetAndSelectState();
            
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1 ~ 4초간 대기
        var changeTime = Random.Range(1, 5);
        
        yield return new WaitForSeconds(changeTime);
        
        // 상태를 "Wander"로 변경
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        _navMeshAgent.speed = _status.WalkSpeed;

        // 목표 위치 설정
        _navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        Vector3 to = new Vector3(_navMeshAgent.destination.x, 0, _navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;
            
            // 목표 위치에 근접하게 도달 또는 너무 오랜 시간 "Wander" 상태에 머무를 시 "Idle" 상태로 변경
            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                ChangeState(EnemyState.Idle);
            }
            
            // 타겟과의 거리에 따라 행동 선택
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        // 현재 위치를 원점으로 하는 원의 반지름
        float wanderRadius = 10;

        // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        var wanderJitter = 0;

        // 최소 각도
        var wanderJitterMin = 0;

        // 최대 각도
        var wanderJitterMax = 360;
        
        // 현재 적 캐릭터가 있는 월드 중심 위치와 크기 (구역을 벗어난 행동 제한)
        Vector3 rangePos = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        // 자신의 위치를 중심으로 반지름 거리, 선택된 각도에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPos = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
        targetPos.x = Mathf.Clamp(
            targetPos.x,
            rangePos.x - rangeScale.x * 0.5f,
            rangePos.x + rangeScale.x * 0.5f
            );
        targetPos.y = 0.0f;
        targetPos.z = Mathf.Clamp(
            targetPos.z,
            rangePos.z - rangeScale.z * 0.5f,
            rangePos.z + rangeScale.z * 0.5f
        );

        return targetPos;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 pos = Vector3.zero;

        pos.x = Mathf.Cos(angle) * radius;
        pos.z = Mathf.Sin(angle) * radius;

        return pos;
    }

    private void OnDrawGizmos()
    {
        // "Wander" 상태일 때 이동할 경로 표시
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, _navMeshAgent.destination - transform.position);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _targetRecognitionRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _pursuitLimitRange);
    }

    private IEnumerator Pursuit()
    {
        while (true)
        {
            // 이동 속도 설정
            _navMeshAgent.speed = _status.RunSpeed;
            
            // 목표위치를 현재 플레이어의 위치로 설정
            _navMeshAgent.SetDestination(_target.position);

            // 타겟 방향을 계속 주시
            LookRotationToTarget();
            
            // 타겟과의 거리에 따라 행동 선택
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        // 목표 위치
        var position = _target.position;
        Vector3 to = new Vector3(position.x, 0, position.z);
        
        // 내 위치
        var position1 = transform.position;
        Vector3 from = new Vector3(position1.x, 0, position1.z);

        // 바로 돌아보기
        transform.rotation = Quaternion.LookRotation(to - from);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (_target == null) return;

        // Target과 적의 거리 계산 후 거리에 따라 행동 선택
        float dis = Vector3.Distance(_target.position, transform.position);

        if (dis <= _targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if (dis >= _pursuitLimitRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }
}
