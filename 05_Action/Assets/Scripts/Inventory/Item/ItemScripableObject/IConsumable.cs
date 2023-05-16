using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 먹는 즉시 사용되는 아이템이 상속받을 인터페이스 
/// </summary>
interface IConsumable
{
    /// <summary>
    /// 아이템을 소비시키는 함수
    /// </summary>
    /// <param name="target">아이템의 효과를 받을 대상</param>
    void Consume(GameObject target);
}