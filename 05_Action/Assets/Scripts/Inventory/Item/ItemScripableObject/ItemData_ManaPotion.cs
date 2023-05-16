using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data - Mana Potion", order = 5)]
public class ItemData_ManaPotion : ItemData, IUsable
{
    [Header("마나 포션 데이터")]

    /// <summary>
    /// 전체 회복량
    /// </summary>
    public float totalRegen = 30.0f;

    /// <summary>
    /// 전체 회복하는데 걸리는 시간
    /// </summary>
    public float duration = 3.0f;

    public bool Use(GameObject target)
    {
        bool result = false;

        if (target != null)
        {
            IMana mana = target.GetComponent<IMana>();
            if(mana != null)
            {
                mana.ManaRegenerate(totalRegen, duration);
                Debug.Log($"{target.name}의 MP가 {duration}초 동안 {totalRegen}만큼 회복됩니다.");
                result = true;
            }
        }

        return result;
    }
}
