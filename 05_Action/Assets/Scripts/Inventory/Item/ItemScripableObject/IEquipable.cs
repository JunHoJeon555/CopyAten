using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IEquipable
{
    /// <summary>
    /// 이 아이템이 장착될 부위
    /// </summary>
    EquipType EquipPart { get; }

    /// <summary>
    /// 아이템 장착하는 함수
    /// </summary>
    /// <param name="target">아이템을 장착받을 대상</param>
    /// <param name="slot">아이템이 들어있는 인벤 슬롯</param>
    void EquipItem(GameObject target, ItemSlot slot);

    /// <summary>
    /// 아이템 장착 해제하는 함수
    /// </summary>
    /// <param name="target">아이템을 장착 해제될 대상</param>
    /// <param name="slot">아이템이 들어있는 인벤 슬롯</param>
    void UnEquipItem(GameObject target, ItemSlot slot);

    /// <summary>
    /// 아이템을 상황에 따라 자동으로 장착하거나 해제하는 함수
    /// </summary>
    /// <param name="target">장착하거나 해제될 대상</param>
    /// <param name="slot">아이템이 들어있는 인벤 슬롯</param>
    void AutoEquipUnequip(GameObject target, ItemSlot slot);

}
