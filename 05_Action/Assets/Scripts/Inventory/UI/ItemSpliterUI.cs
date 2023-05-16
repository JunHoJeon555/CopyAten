using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpliterUI : MonoBehaviour
{
    /// <summary>
    /// 아이템 분리할 최소 갯수(나누기를 시도하면 최소 1개는 나눈다.)
    /// </summary>
    const int itemCountMin = 1;

    /// <summary>
    /// 아이템을 실제로 분리할 갯수
    /// </summary>
    uint itemSplitCount = itemCountMin;
    uint ItemSplitCount
    {
        get => itemSplitCount;
        set
        {
            // 숫자 입력됬을 때 1~최대치까지로 조절
            itemSplitCount = (uint)Mathf.Clamp((int)value, itemCountMin, (int)(targetSlot.ItemCount - 1));

            // 인풋필드와 슬라이더에도 반영
            inputField.text = itemSplitCount.ToString();
            slider.value = itemSplitCount;
        }
    }

    /// <summary>
    /// 아이템을 분리할 슬롯
    /// </summary>
    ItemSlot targetSlot;

    /// <summary>
    /// 아이템을 분리할 갯수를 직접 입력할 수 있는 인풋 필드
    /// </summary>
    TMP_InputField inputField;

    /// <summary>
    /// 아이템 분리 갯수를 조절할 수 있는 슬라이더
    /// </summary>
    Slider slider;

    /// <summary>
    /// 분리할 아이템의 아이콘
    /// </summary>
    Image itemImage;

    /// <summary>
    /// OK버튼을 눌렀을 때 실행되는 함수
    /// 파라메터(슬롯의 인덱스, 나눌 갯수)
    /// </summary>
    public Action<uint, uint> onOKClick;

    private void Awake()
    {
        // 인풋 필드
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.onValueChanged.AddListener((text) =>
        {
            if( uint.TryParse(text, out uint result) )
            {
                ItemSplitCount = result;
            }
            else
            {
                ItemSplitCount = itemCountMin;
            }
        });

        // 슬라이더
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener((value) => ItemSplitCount = (uint)value);

        // 아이템 아이콘 이미지
        Transform child = transform.GetChild(0);
        itemImage = child.GetComponent<Image>();

        // 더하기 버튼
        child = transform.GetChild(2);
        Button plus = child.GetComponent<Button>();
        plus.onClick.AddListener(() => ItemSplitCount++);

        // 마이너스 버튼
        child = transform.GetChild(3);
        Button minus = child.GetComponent<Button>();
        minus.onClick.AddListener(() => ItemSplitCount--);

        // OK 버튼
        child = transform.GetChild(5);
        Button ok = child.GetComponent<Button>();
        ok.onClick.AddListener(() =>
        {
            // targetSlot.Index 슬롯에서 ItemSplitCount만큼 덜어내라고 알림
            onOKClick?.Invoke(targetSlot.Index, ItemSplitCount);
            Close();
        });

        // 취소 버튼
        child = transform.GetChild(6);
        Button cancel = child.GetComponent<Button>();
        cancel.onClick.AddListener(Close);
    }

    /// <summary>
    /// 아이템 분리창을 여는 함수
    /// </summary>
    /// <param name="target">아이템을 분리할 슬롯</param>
    public void Open(ItemSlot target)
    {
        if(target.ItemCount > itemCountMin) // 최소치보다 클때만 분리작업 수행
        {
            targetSlot = target;                    // 슬롯 저장
            ItemSplitCount = itemCountMin;          // 기본 값 설정
            itemImage.sprite = targetSlot.ItemData.itemIcon;    // 아이콘 설정
            slider.minValue = itemCountMin;         // 슬라이더 범위 지정
            slider.maxValue = target.ItemCount - 1;
            gameObject.SetActive(true);             // 보여주기
        }
    }

    /// <summary>
    /// 아이템 분리창을 닫는 함수
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
