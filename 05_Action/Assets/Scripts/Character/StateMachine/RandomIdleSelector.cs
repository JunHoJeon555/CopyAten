using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleSelector : StateMachineBehaviour
{
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if( !animator.IsInTransition(layerIndex))  // layerIndex번째 레이어가 트랜지션 중이 아닐때 아래 코드 실행
        {
            animator.SetInteger("IdleSelect", RandomSelect());
        }
    }    

    int RandomSelect()
    {
        // 0 ~ 4 중에서 랜덤으로 숫자를 골라 리턴하기
        // 각 번호별로 확률이 달라야 한다.
        // 0: 40%
        // 1: 15%
        // 2: 15%
        // 3: 15%
        // 4: 15%

        float num = Random.Range(0f, 1f);
        int select;
        if(num < 0.4f)
        {
            select = 0;
        }
        else if(num < 0.55f)
        {
            select = 1;
        }
        else if (num < 0.7f)
        {
            select = 2;
        }
        else if (num < 0.85f)
        {
            select = 3;
        }
        else
        {
            select = 4;
        }

        return select;
    }
}
