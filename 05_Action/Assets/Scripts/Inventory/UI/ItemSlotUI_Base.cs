using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemSlotUI_Base : MonoBehaviour
{
    /// <summary>
    /// 인벤토리의 몇번째 슬롯과 연결되었는지. 이 슬롯이 몇번째 슬롯인지.
    /// </summary>
    uint id;
    public uint ID => id;

    /// <summary>
    /// 이 UI와 연결된 ItemSlot
    /// </summary>
    ItemSlot itemSlot;
    public ItemSlot ItemSlot => itemSlot;

    /// <summary>
    /// 아이템 아이콘 표시용 이미지
    /// </summary>
    Image itemImage;

    /// <summary>
    /// 아이템 갯수 표시용 텍스트
    /// </summary>
    TextMeshProUGUI itemCount;

    protected virtual void Awake()
    {
        Transform child = transform.GetChild(0);
        itemImage = child.GetComponent<Image>();
        child = transform.GetChild(1);
        itemCount = child.GetComponent<TextMeshProUGUI>();
    }
    
    /// <summary>
    /// 슬롯 초기화용 함수
    /// </summary>
    /// <param name="id">슬롯 인덱스. 슬롯 ID의 역할도 함</param>
    /// <param name="slot">이 UI가 보여줄 ItemSlot</param>
    public virtual void InitializeSlot(uint id, ItemSlot slot)
    {
        //Debug.Log($"{id} 슬롯 초기화");
        this.id = id;       // 값 설정
        itemSlot = slot;
        itemSlot.onSlotItemChange = Refresh;    // 슬롯에 들어있는 아이템이 변경되었을 때 실행될 함수 등록

        Refresh();          // 보이는 모습 초기화
    }

    /// <summary>
    /// 슬롯의 보이는 모습 갱신용 함수
    /// itemSlot에 들어있는 아이템이 변경될 때마다 실행.
    /// </summary>
    private void Refresh()
    {
        if (itemSlot.IsEmpty)
        {
            // 슬롯에 아이템이 들어있지 않을 때
            itemImage.sprite = null;        // 이미지 제거하고
            itemImage.color = Color.clear;  // 투명하게 만들고
            itemCount.text = string.Empty;  // 갯수도 비우기
        }
        else
        {
            // 슬롯에 아이템이 들어있을 때
            itemImage.sprite = itemSlot.ItemData.itemIcon;      // 이미지 설정하고
            itemImage.color = Color.white;                      // 불투명하게 만들기
            itemCount.text = ItemSlot.ItemCount.ToString();     // 갯수 글자로 넣기
        }
        OnRefresh();
    }

    protected virtual void OnRefresh()
    {
    }
}
