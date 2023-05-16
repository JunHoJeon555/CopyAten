using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data - Food", order = 3)]
public class ItemData_Food : ItemData, IConsumable
{
    [Header("음식 아이템 데이터")]

    /// <summary>
    /// 전체 회복량
    /// </summary>
    public float healAmount;

    /// <summary>
    /// 전체 회복 시간
    /// </summary>
    public float duration;

    public void Consume(GameObject target)
    {
        IHealth health = target.GetComponent<IHealth>();
        if (health != null)
        {
            health.HealthRegenerate(healAmount, duration);            
        }
    }
}
