using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data - Drink", order = 4)]
public class ItemData_Drink : ItemData, IConsumable
{
    [Header("음료 아이템 데이터")]

    /// <summary>
    /// 전체 회복량
    /// </summary>
    public float manaAmount;

    /// <summary>
    /// 전체 회복 시간
    /// </summary>
    public float duration;

    public void Consume(GameObject target)
    {
        IMana mana = target.GetComponent<IMana>();
        if (mana != null)
        {
            mana.ManaRegenerate(manaAmount, duration);            
        }
    }
}
