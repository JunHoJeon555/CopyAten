using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData_Equip : ItemData, IEquipable
{
    [Header("장비 아이템 데이터")]
    /// <summary>
    /// 아이템을 장비했을 때 보일 오브젝트의 프리팹
    /// </summary>
    public GameObject equipPrefab;

    /// <summary>
    /// 장착될 위치를 알려주는 프로퍼티
    /// </summary>
    public virtual EquipType EquipPart => EquipType.Weapon; // 인터페이스를 구현해야 하기 때문에 만든 것

    /// <summary>
    /// 아이템 장비하는 함수
    /// </summary>
    /// <param name="target">아이템을 장비할 대상</param>
    /// <param name="slot">아이템이 들어있는 슬롯</param>
    public void EquipItem(GameObject target, ItemSlot slot)
    {
        if(target != null)
        {
            IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();
            if( equipTarget != null)
            {
                equipTarget.EquipItem(EquipPart, slot);
                Debug.Log($"{slot.Index}번째 슬롯 아이템 장착");
            }
        }
    }

    /// <summary>
    /// 아이템 장비 해제하는 함수
    /// </summary>
    /// <param name="target">아이템을 장비할 대상</param>
    /// <param name="slot">아이템이 들어있는 슬롯</param>
    public void UnEquipItem(GameObject target, ItemSlot slot)
    {
        if (target != null)
        {
            IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();
            if (equipTarget != null)
            {                
                equipTarget.UnEquipItem(EquipPart);
                Debug.Log($"{slot.Index}번째 슬롯 아이템 해제");
            }
        }
    }

    /// <summary>
    /// 상황에 따라 아이템을 장비하고 해제하는 함수
    /// </summary>
    /// <param name="target">아이템을 장비할 대상</param>
    /// <param name="slot">아이템이 들어있는 슬롯</param>
    public void AutoEquipUnequip(GameObject target, ItemSlot slot)
    {
        if (target != null)
        {
            IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();
            if (equipTarget != null)    // 대상이 아이템을 장비할 수 있는지 확인
            {
                ItemSlot oldSlot = equipTarget[EquipPart];
                if(oldSlot == null)                 // 이미 장비된 것이 있는지 확인
                {
                    // 장비된 것이 없으면
                    EquipItem(target, slot);        // 새 슬롯에 들어있는 파츠 장착하기
                }
                else
                {
                    // 장비된 것이 있으면 
                    UnEquipItem(target, oldSlot);   // 우선 옛날 슬롯에 있던 아이템은 해제
                    if( oldSlot != slot )
                    {
                        EquipItem(target, slot);    // 새슬롯과 옛슬롯이 다른 종류의 아이템이면 새로 장착
                    }
                }
            }
        }
    }
}
