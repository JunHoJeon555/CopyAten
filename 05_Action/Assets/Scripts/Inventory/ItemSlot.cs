using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    /// <summary>
    /// 이 슬롯의 인덱스(인벤토리에서 몇번째 슬롯인지)
    /// </summary>
    uint slotIndex;
    public uint Index => slotIndex;

    /// <summary>
    /// 이 슬롯에 들어있는 아이템 종류
    /// </summary>
    ItemData slotItemData = null;
    public ItemData ItemData
    {
        get => slotItemData;
        private set     // 쓰기는 이 클래스만 가능
        {
            if( slotItemData != value ) // 아이템 종류가 변경되었을 때만 대입
            {
                slotItemData = value;
                onSlotItemChange?.Invoke();
            }
        }
    }
    public Action onSlotItemChange;
    /// <summary>
    /// 슬롯이 비었는지 확인하는 프로퍼티. true면 비어있고 false면 아이템이 들어있다.
    /// </summary>
    public bool IsEmpty => slotItemData == null;

    /// <summary>
    /// 이 슬롯에 들어있는 아이템의 갯수
    /// </summary>
    uint itemCount = 0;
    public uint ItemCount
    {
        get => itemCount;
        private set
        {
            if( itemCount != value )
            {
                itemCount = value;
                onSlotItemChange?.Invoke();
            }
        }
    }

    /// <summary>
    /// 이 슬롯의 아이템이 장비되었는지 여부
    /// </summary>
    bool isEquipped = false;
    public bool IsEquipped
    {
        get => isEquipped;
        set
        {
            isEquipped = value;
            onSlotItemChange?.Invoke();
        }
    }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="index">이 슬롯이 인벤토리에서 몇번째 슬롯인지(인덱스)</param>
    public ItemSlot(uint index)
    {
        slotIndex = index;  // slotIndex는 이 이후로 절대 변경하지 않아야 한다.
        ItemCount = 0;
        IsEquipped = false;
    }

    /// <summary>
    /// 아이템 설정하는 함수
    /// </summary>
    /// <param name="data">설정할 아이템 종류</param>
    /// <param name="count">설정할 아이템 갯수</param>
    public void AssignSlotItem(ItemData data, uint count = 1)
    {
        if( data !=  null )
        {
            ItemData = data;    // data가 null이 아니면 파라메터로 설정
            ItemCount = count;
            IsEquipped = false;
            Debug.Log($"인벤토리 {slotIndex}슬롯에 \"{ItemData.itemName}\" 아이템이 {ItemCount}개 설정");
        }
        else
        {
            ClearSlotItem();    // data가 null이면 해당 슬롯은 초기화
        }
    }

    // 아이템 비우기
    public void ClearSlotItem()
    {
        ItemData = null;
        ItemCount = 0;
        isEquipped = false;
        Debug.Log($"인벤토리 {slotIndex}슬롯을 비웁니다.");
    }

    /// <summary>
    /// 아이템 갯수 증가시키는 함수
    /// </summary>
    /// <param name="overCount">추가하다가 넘친 갯수(out)</param>
    /// <param name="increaseCount">증가시킬 갯수</param>
    /// <returns>증가 성공 여부(true면 성공, false면 넘쳤다)</returns>
    public bool IncreaseSlotItem(out uint overCount, uint increaseCount = 1)
    {
        bool result;
        int over;

        // 넘치느냐 안넘치느냐를 판별하기
        uint newCount = ItemCount + increaseCount;
        over = (int)newCount - (int)ItemData.maxStackCount;

        if(over > 0)
        {
            // 넘친 경우에 대한 처리
            ItemCount = ItemData.maxStackCount;
            overCount = (uint)over;
            result = false;
            Debug.Log($"인벤토리 {slotIndex}번 슬롯에 " +
                $"\"{ItemData.itemName}\"아이템이 최대치까지 증가. " +
                $"현재 {ItemCount}개. {over}개 넘침.");
        }
        else
        {
            // 안 넘친 경우에 대한 처리
            ItemCount = newCount;
            overCount = 0;
            result = true;
            Debug.Log($"인벤토리 {slotIndex}번 슬롯에 " +
                $"\"{ItemData.itemName}\"아이템이 {increaseCount}개 증가. " +
                $"현재 {ItemCount}개.");
        }

        return result;
    }

    /// <summary>
    /// 아이템 갯수 감소시키는 함수
    /// </summary>
    /// <param name="decreaseCount">감소시킬 아이템 갯수</param>
    public void DecreaseSlotItem(uint decreaseCount = 1)
    {
        int newCount = (int)ItemCount - (int)decreaseCount; // 언더플로우 대비해서 int로 변경 후 빼기

        if( newCount < 1 )
        {
            // 새 갯수가 0개 이하
            ClearSlotItem();
        }
        else
        {
            // 갯수가 남아있음.
            ItemCount = (uint)newCount;
            Debug.Log($"인벤토리 {slotIndex}번 슬롯에 " +
                $"\"{ItemData.itemName}\"아이템이 {decreaseCount}개 감소. " +
                $"현재 {ItemCount}개.");
        }
    }


    /// <summary>
    /// 아이템을 사용하는 함수
    /// </summary>
    /// <param name="target">아이템의 효과를 받을 대상</param>
    public void UseItem(GameObject target)
    {
        IUsable usable = ItemData as IUsable;
        if( usable != null )            // 사용 가능한 아이템이면
        {
            if(usable.Use(target))      // 사용 시도하고
            {
                DecreaseSlotItem();     // 사용에 성공하면 갯수 하나 감소
            }
        }
    }

    /// <summary>
    /// 아이템을 장비/해제하는 함수
    /// </summary>
    /// <param name="target">아이템을 장비하거나 해제할 대상</param>
    public void EquipItem(GameObject target)
    {
        IEquipable equipable = ItemData as IEquipable;
        if( equipable != null )                         // 장비가능한 아이템이면
        {
            equipable.AutoEquipUnequip(target, this);   // 상황에 따라 장비하거나 해제하기
        }
    }
}
