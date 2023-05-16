using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    /// <summary>
    /// 자신의 transform(공격할 때 바라봐야 하니까)
    /// </summary>
    Transform transform { get; }

    /// <summary>
    /// 공격력 확인용 프로퍼티
    /// </summary>
    float AttackPower { get; }

    /// <summary>
    /// 방어력 확인용 프로퍼티
    /// </summary>
    float DefencePower { get; }

    /// <summary>
    /// 공격하는 함수
    /// </summary>
    /// <param name="target">공격할 대상</param>
    void Attack(IBattle target);

    /// <summary>
    /// 방어하는 함수
    /// </summary>
    /// <param name="damage">내가 받은 대미지</param>
    void Defence(float damage);
}
