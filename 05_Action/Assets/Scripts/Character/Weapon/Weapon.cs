using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 공격했을 때 나올 이팩트 프리팹
    /// </summary>
    public GameObject hitEffect;

    /// <summary>
    /// 칼날의 역할을 할 컬라이더. 특정 타이밍에만 활성화
    /// </summary>
    CapsuleCollider blade;

    /// <summary>
    /// 칼 이팩트용 파티클 시스템
    /// </summary>
    ParticleSystem ps;

    Player player;

    private void Awake()
    {
        blade = GetComponent<CapsuleCollider>();
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        player = GameManager.Inst.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log($"{other.name}을 공격했다.");
            IBattle target = other.GetComponent<IBattle>();
            if(target != null)
            {
                player.Attack(target);
            }

            // 공격을 했을 때 적이 맞으면 맞은 위치에 hitEffect 생성하기
            Vector3 impactPoint = transform.position + transform.up;    // 무기의 임팩트 지점(대략적으로 구함)
            Vector3 effectPoint = other.ClosestPoint(impactPoint);  // 무기 임팩트 지점과 가장 가까운 지점(컬라이더의 표면)

            Instantiate(hitEffect, effectPoint, Quaternion.identity);
        }
    }

    /// <summary>
    /// 칼날을 켜고 끄는 함수. 플레이어의 델리게이트로 실행됨
    /// </summary>
    /// <param name="enable">true면 활성화, false면 비활성화</param>
    public void ColliderEnable(bool enable)
    {
        blade.enabled = enable;
    }

    /// <summary>
    /// 칼 이팩트 켜고 끄는 함수. 플레이어의 델리게이트로 실행됨
    /// </summary>
    /// <param name="play">true면 활성화, false면 비활성화</param>
    public void EffectPlay(bool play)
    {
        if(play)
        {
            ps.Play();
        }
        else
        {
            ps.Stop();
        }
    }
}
