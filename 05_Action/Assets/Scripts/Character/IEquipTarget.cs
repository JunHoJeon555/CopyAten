using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IEquipTarget
{
    /// <summary>
    /// 장비 아이템의 부위별 슬롯 확인용 인덱서
    /// </summary>
    /// <param name="part">확인할 파츠</param>
    /// <returns>장비되어있지 않으면 null</returns>
    ItemSlot this[EquipType part] { get; }

    /// <summary>
    /// 아이템을 장비하는 함수
    /// </summary>
    /// <param name="part">장비할 파츠</param>
    /// <param name="slot">장비할 아이템이 들어있는 슬롯</param>
    void EquipItem(EquipType part, ItemSlot slot);

    /// <summary>
    /// 아이템을 장비 해제하는 함수
    /// </summary>
    /// <param name="part">장비해제 할 파츠</param>
    void UnEquipItem(EquipType part);

    /// <summary>
    /// 아이템이 장비될 트랜스폼의 부모를 돌려주는 함수
    /// </summary>
    /// <param name="part">장비될 부위</param>
    /// <returns>장비될 부위의 부모 트랜스폼</returns>
    Transform GetPartTransform(EquipType part);
}
