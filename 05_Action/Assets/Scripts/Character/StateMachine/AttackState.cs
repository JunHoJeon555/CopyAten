using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// 공격 상태에서 실행되는 스크립트
/// </summary>
public class AttackState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 무조건 무기 보이게 하기
        GameManager.Inst.Player.ShowWeaponAndShield(true);

        // 공격 시작할 때 무기 이팩트 켜기
        GameManager.Inst.Player.WeaponEffectEnable(true);   
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 공격 끝날 때 무기 이팩트 끄기
        GameManager.Inst.Player.WeaponEffectEnable(false);
    }
}
