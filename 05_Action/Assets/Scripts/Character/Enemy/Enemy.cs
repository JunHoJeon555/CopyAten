using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour, IBattle, IHealth
{
    // 순찰
    // - 정해진 웨이포인트 지점을 반복해서 이동
    // - 웨이포인트 지점에 도착하면 일정 시간 정지

    // 추적
    // - 순찰 중에 플레이어 발견하면 추적
    // - 시야 범위에서 플레이어가 벗어나면 다시 순찰

    // 공격
    // - 플레이어가 공격 범위안에 들어오면 공격 시작

    // 사망
    // - 플레이어에게 일정 이상 데미지를 입으면 사망
    // - 사망하면 파티클 이팩트 재생

    protected enum EnemyState
    {
        WaitState = 0,   // 대기 상태
        PatrolState,     // 순찰 상태
        ChaseState,      // 추적 상태
        AttackState,     // 공격 상태
        DieState         // 죽은 상태
    }

    /// <summary>
    /// 현재 적의 상태
    /// </summary>
    EnemyState state = EnemyState.PatrolState;   // Start할 때 wait로 설정하기 위해 임시로 설정

    /// <summary>
    /// 상태를 확인하고 변경시 일어나는 처리를 하는 프로퍼티
    /// </summary>
    EnemyState State
    {
        get => state;
        set
        {
            if (state != value)     // 상태가 변경될 때만 실행
            {
                state = value;
                switch (state)      // 변경된 상태에 따라 서로 다른 처리를 수행
                { 
                    case EnemyState.WaitState:
                        // WaitState 상태가 될 때 처리해야 할 일들
                        agent.isStopped = true;         // 길찾기로 움직이던 것 정지
                        agent.velocity = Vector3.zero;  // 길찾기 관성 제거
                        anim.SetTrigger("Stop");        // Idle 애니메이션 재생
                        WaitTimer = waitDuration + UnityEngine.Random.Range(0.0f, 0.5f);    // 기다릴 시간 설정
                        stateUpdate = Update_Wait;      // WaitState 상태용 업데이트 함수 설정
                        break;
                    case EnemyState.PatrolState:
                        // PatrolState 상태가 될 때 처리해야 할 일들
                        agent.isStopped = false;        // 길찾기 정지를 해제(다시 움직일 수 있게 설정)
                        agent.SetDestination(moveTarget.position);  // 움직일 목적지 설정
                        anim.SetTrigger("Move");        // 이동 애니메이션 재생
                        stateUpdate = Update_Patrol;    // PatrolState 상태용 업데이트 함수 설정
                        break;
                    case EnemyState.ChaseState:
                        // ChaseState 상태가 될 때 처리해야 할 일들
                        agent.isStopped = false;        // 길찾기 정지를 해제(다시 움직일 수 있게 설정)
                        agent.SetDestination(chaseTarget.position); // 추적 대상으로 이동하게 설정
                        anim.SetTrigger("Move");        // 이동 애니메이션 재생
                        stateUpdate = Update_Chase;     // ChaseState 상태용 업데이트 함수 설정
                        break;
                    case EnemyState.AttackState:
                        break;
                    case EnemyState.DieState:
                        agent.isStopped = true;     // 길찾기 정지
                        agent.velocity = Vector3.zero;                        
                        anim.SetTrigger("Die");     // 사망 애니메이션 재생   
                        stateUpdate = Update_Die;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 상태별 Update함수를 저장할 델리게이트
    /// </summary>
    Action stateUpdate = null;

    // 순찰 관련 데이터 ----------------------------------------------------------------------------

    [Header("순찰 데이터")]
    /// <summary>
    /// 순찰할 웨이포인트 모음
    /// </summary>
    public Waypoints waypoints;

    /// <summary>
    /// 지금 이동할 목적지
    /// </summary>
    Transform moveTarget;

    // 대기 관련 데이터 ----------------------------------------------------------------------------

    [Header("대기 데이터")]
    /// <summary>
    /// 목적지에 도달하면 기다리는 시간
    /// </summary>
    public float waitDuration = 1.0f;

    /// <summary>
    /// 실제로 기다릴 시간
    /// </summary>
    float waitTimer = 0.0f;
    float WaitTimer
    {
        get => waitTimer;
        set
        {
            waitTimer = value;
            if( waypoints != null && waitTimer < 0.0f)  // waitTimer이 0보다 작아지면 순찰 상태로 전환
            {
                State = EnemyState.PatrolState;
            }
        }
    }

    // 추적 관련 데이터 ----------------------------------------------------------------------------
    [Header("추적 데이터")]
    /// <summary>
    /// 전체 시야 범위
    /// </summary>
    public float sightRange = 10.0f;

    /// <summary>
    /// 시야각의 절반
    /// </summary>
    public float sightHalfAngle = 50.0f;

    /// <summary>
    /// 근접 시야 범위
    /// </summary>
    public float closeSightRange = 2.5f;

    /// <summary>
    /// 추적할 대상의 트랜스폼
    /// </summary>
    Transform chaseTarget;

    /// <summary>
    /// 추적중이면 true, 아니면 false
    /// </summary>
    bool IsChasing => chaseTarget != null;

    // 전투 관련 데이터 ----------------------------------------------------------------------------
    [Header("전투 데이터")]
    public float attackPower = 10.0f;
    public float AttackPower => attackPower;

    public float defencePower = 2.0f;
    public float DefencePower => defencePower;

    public bool IsAlive => hp > 0;

    public float maxHP = 100.0f;
    float hp = 100.0f;
    public float HP 
    { 
        get => hp;
        set
        {
            hp = value;
            if( State != EnemyState.DieState && hp <= 0 ) 
            {
                Die();
            }
            hp = Mathf.Clamp(hp, 0.0f, maxHP);
            onHealthChange?.Invoke(hp / maxHP);
        }
    }

    public float MaxHP => maxHP;

    public Action<float> onHealthChange { get; set; }
    public Action onDie { get; set; }


    // 아이템 드랍 관련 데이터 ----------------------------------------------------------------------
    [System.Serializable]
    public struct ItemDropInfo          // 아이템 드랍 정보
    {
        public ItemCode code;           // 드랍할 아이템 종류
        [Range(0.0f, 1.0f)]
        public float dropPercentage;    // 드랍될 확률
    }

    [Header("드랍 데이터")]
    public ItemDropInfo[] dropItems;    // 이 슬라임이 죽을 때 드랍할 수 있는 모든 아이템 정보


    // 컴포넌트 ------------------------------------------------------------------------------------

    NavMeshAgent agent;
    Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        Waypoints defaultWaypoints = GetComponentInChildren<Waypoints>();
        defaultWaypoints.transform.SetParent(null);
        if ( waypoints == null ) 
        {
            waypoints = defaultWaypoints;   // 따로 설정한 웨이포인트가 없으면 자식으로 붙어있는 웨이포인트 사용
        }
    }

    private void Start()
    {
        moveTarget = waypoints.Current;
        State = EnemyState.WaitState;
        anim.ResetTrigger("Stop");          // 첫 WaitState 상태로 가면서 Stop 트리거가 미리 설정되는 것 방지
    }

    private void Update()
    {
        stateUpdate();      // 현재 상태의 Update 함수 수행
    }

    /// <summary>
    /// 순찰용 Update 함수
    /// </summary>
    void Update_Patrol()
    {
        if (SearchPlayer())
        {
            // 플레이어 발견하면 즉시 추적상태로 변경
            State = EnemyState.ChaseState;
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 목적지에 도착했다.
            moveTarget = waypoints.Next();  // 도착하면 다음 지점 설정해 놓고
            State = EnemyState.WaitState;        // 대기 상태로 변경
        }
    }

    /// <summary>
    /// 대기용 Update 함수
    /// </summary>
    void Update_Wait()
    {
        if (SearchPlayer())
        {
            // 플레이어 발견하면 즉시 추적상태로 변경
            State = EnemyState.ChaseState;
        }
        else
        {
            // 그냥 기다리고 있는 상황이면
            WaitTimer -= Time.deltaTime;    // 타이머만 계속 감소
        }
    }

    /// <summary>
    /// 추적용 Update 함수
    /// </summary>
    void Update_Chase()
    {
        if (SearchPlayer())
        {
            // 플레이어 발견하면 즉시 추적상태로 변경
            State = EnemyState.ChaseState;
            agent.SetDestination(chaseTarget.position); // state는 같은 값이면 수행을 안함.
        }
        else
        {
            // 안보이면 대기 상태로
            State = EnemyState.WaitState;
        }
    }

    private void Update_Die()
    {
    }

    /// <summary>
    /// 플레이어를 감지하는 함수
    /// </summary>
    /// <returns>찾았으면 true, 못찾았으면 false</returns>
    bool SearchPlayer()
    {
        bool result = false;
        chaseTarget = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        if(colliders.Length > 0 )
        {
            Vector3 playerPos = colliders[0].transform.position;
            Vector3 toPlayerDir = playerPos - transform.position;
            if(toPlayerDir.sqrMagnitude < closeSightRange * closeSightRange)
            {
                // 근접 시야 범위 안에 플레이어가 있다.
                chaseTarget = colliders[0].transform;
                result = true;
            }
            else
            {
                // 전체 시야 범위 안에 플레이어가 있다.
                float angle = Vector3.Angle(transform.forward, toPlayerDir);
                if(angle < sightHalfAngle)
                {
                    // 시야각 안에 플레이어가 있다.
                    Ray ray = new Ray(transform.position + transform.up * 0.5f, toPlayerDir);
                    if( Physics.Raycast(ray, out RaycastHit hit, sightRange))
                    {
                        // 시야에 부딪친 물체가 있다.
                        if( hit.collider.CompareTag("Player"))
                        {
                            // 부딪친 물체가 플레이어이다.
                            chaseTarget = colliders[0].transform;
                            result = true;
                        }
                    }
                }
            }
        }

        return result;
    }

    public void Attack(IBattle target)
    {
        target?.Defence(AttackPower);
        Debug.Log($"{target.transform.gameObject.name}에게 {attackPower}만큼 공격을 했습니다.");
    }

    public void Defence(float damage)
    {
        if( State != EnemyState.DieState )
        {
            Debug.Log($"{Mathf.Max(1, damage - DefencePower)}만큼의 데미지를 입었습니다. ({HP})");
            anim.SetTrigger("Hit");
            HP -= Mathf.Max(1, damage - DefencePower);
        }
    }

    public void Die()
    {
        State = EnemyState.DieState;
        onDie?.Invoke();
        MakeDropItem();
        Debug.Log($"{this.gameObject.name}이 죽었습니다.");
    }

    public void HealthRegenerate(float totalRegen, float duration)
    {
        // 아무 동작 안함
    }

    void MakeDropItem()
    {
        foreach(var item in dropItems)
        {
            if( item.dropPercentage > UnityEngine.Random.value )
            {
                ItemFactory.MakeItem(item.code, transform.position, true);
            }
        }
    }

    public void Test_ItemDrop()
    {
        MakeDropItem();
    }

    public void Test_DropCheck(int testCount)
    {
        int[] counts = new int[Enum.GetValues(typeof(ItemCode)).Length];
        
        for(int i=0;i<testCount;i++)
        {
            foreach (var item in dropItems)
            {
                if (item.dropPercentage > UnityEngine.Random.value)
                {
                    counts[(int)item.code]++;
                }
            }
        }

        int code = 0;
        foreach (var itemCount in counts)
        {
            if(itemCount > 0)
            {
                Debug.Log($"{(ItemCode)code} : {itemCount}");
            }
            code++;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 원 그리기 : Handles.DrawWireDisc

        // 호 그리기(중심점, 중심축의 벡터, 시작 백터, 부채꼴의 각도, 반지름, 호의 두께)
        // Handles.DrawWireArc(transform.position, transform.up, transform.forward, 90.0f, 10.0f, 5.0f);

        Handles.color = Color.green;        // 기본 색은 녹색
        if( IsChasing )
        {
            Handles.color = Color.red;      // 시야 범위안에 플레이어가 있으면 빨간색으로 그리기
        }

        // 원거리 시야 그리기
        Vector3 eyePosition = transform.position + transform.up * 0.5f; // 눈의 위치 계산

        Vector3 forward = transform.forward * sightRange;                   // sightRange만큼 앞으로 나가는 방향
        Handles.DrawDottedLine(eyePosition, eyePosition + forward, 2.0f);   // eye에서 eye 앞쪽 위치까지 선긋기

        // 적의 up벡터를 축으로 왼쪽방향으로 sightHalfAngle만큼 회전 시키는 회전
        Quaternion q1 = Quaternion.AngleAxis(-sightHalfAngle, transform.up);
        // 적의 up벡터를 축으로 오른쪽방향으로 sightHalfAngle만큼 회전 시키는 회전
        Quaternion q2 = Quaternion.AngleAxis(sightHalfAngle, transform.up);     

        Vector3 p1 = q1 * forward;  // eye 앞쪽 방향을 왼쪽으로 회전
        Vector3 p2 = q2 * forward;  // eye 앞쪽 방향을 오른쪽으로 회전

        Handles.DrawLine(eyePosition, eyePosition + p1);    // 눈의 위치에서 회전시킨 위치로 선 긋기
        Handles.DrawLine(eyePosition, eyePosition + p2);

        Handles.DrawWireArc(eyePosition, transform.up, p1, sightHalfAngle * 2, sightRange, 3.0f);   // 호 그리기

        Handles.color = Color.yellow;       // 근접 시야 기본 색상
        if (IsChasing)
        {
            Handles.color = Color.red;      // 추적 중이면 빨간 색으로 변경
        }
        
        Handles.DrawWireDisc(transform.position, transform.up, closeSightRange, 3.0f);  // 근접 시야 그리기
    }
#endif
}
