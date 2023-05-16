using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data - Healing Potion", order = 4)]
public class ItemData_HealingPotion : ItemData, IUsable
{
    [Header("힐링 포션 데이터")]
    public float heal = 20.0f;

    public bool Use(GameObject target)
    {
        bool result = false;

        if (target != null)
        {
            IHealth health = target.GetComponent<IHealth>();
            if(health != null)
            {
                health.HP += heal;
                Debug.Log($"{target.name}의 HP가 {heal}만큼 증가해서 {health.HP}가 되었습니다.");
                result = true;
            }
        }

        return result;
    }
}
