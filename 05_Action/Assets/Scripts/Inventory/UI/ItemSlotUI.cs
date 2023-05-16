using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotUI : ItemSlotUI_Base, IDragHandler, IBeginDragHandler, IEndDragHandler, 
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    TextMeshProUGUI equipped;

    /// <summary>
    /// 드래그 시작을 알리는 델리게이트
    /// </summary>
    public Action<uint> onDragBegin;

    /// <summary>
    /// 드래그 종료를 알리는 델리게이트
    /// </summary>
    public Action<uint, bool> onDragEnd;

    /// <summary>
    /// 슬롯이 클릭되었을 때 실행되는 델리게이트
    /// </summary>
    public Action<uint> onClick;

    /// <summary>
    /// 슬롯에 마우스가 들어왔을 때 실행되는 델리게이트
    /// </summary>
    public Action<uint> onPointerEnter;

    /// <summary>
    /// 슬롯에서 마우스가 나갔을 때 실행되는 델리게이트
    /// </summary>
    public Action<uint> onPointerExit;

    /// <summary>
    /// 슬롯 위에서 마우스가 움직일 때 실행되는 델리게이트
    /// </summary>
    public Action<Vector2> onPointerMove;


    protected override void Awake()
    {
        base.Awake();
        Transform child = transform.GetChild(2);
        equipped = child.GetComponent<TextMeshProUGUI>();
    }


    /// <summary>
    /// 이 슬롯UI를 초기화하는 함수
    /// </summary>
    /// <param name="id">이 슬롯UI의 ID</param>
    /// <param name="slot">이 슬롯UI가 보여줄 ItemSlot</param>
    public override void InitializeSlot(uint id, ItemSlot slot)
    {
        // 델리게이트에 이전 영향 제거하기
        onDragBegin = null; 
        onDragEnd = null;
        onClick = null;
        onPointerEnter = null;
        onPointerExit = null;
        onPointerMove = null;

        // 부모가 처리하는 것
        base.InitializeSlot(id, slot);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"드래그 시작 : {ID}번 슬롯");
        onDragBegin?.Invoke(ID);    // 델리게이트로 이 슬롯에서 드래그가 시작되었음을 알림
    }

    public void OnDrag(PointerEventData eventData)
    {
        //OnBeginDrag와 OnEndDrag를 위해 추가한 것
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;    // 드래그가 끝난 지점의 게임 오브젝트 가져오기
        if( obj != null)
        {
            // 오브젝트가 있으면 
            ItemSlotUI endSlot = obj.GetComponent<ItemSlotUI>();
            if( endSlot != null )   // 슬롯인지 확인
            {
                // 슬롯이면 델리게이트로 이 슬롯에서 드래그가 끝났음을 알림
                onDragEnd?.Invoke(endSlot.ID, true);
                Debug.Log($"드래그 종료 : {endSlot.ID}번 슬롯");
            }
            else
            {
                onDragEnd?.Invoke(ID, false);
                Debug.Log("슬롯이 아닙니다. 원래 슬롯으로 되돌립니다.");
            }
        }
        else
        {
            Debug.Log("오브젝트가 없습니다.");
        }
    }

    /// <summary>
    /// 슬롯을 클릭했을 때 실행되는 함수.
    /// 임시 슬롯에 있는 아이템을 이 슬롯에 넣기 위한 용도
    /// 아이템 분리하기 위한 용도
    /// 아이템 사용하기 위한 용도
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(ID);    // 클릭되었다고 신호만 보내기
    }

    /// <summary>
    /// 상세 정보창 열고 닫는 것이 주 목적
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(ID);
    }

    /// <summary>
    /// 상세 정보창 열고 닫는 것이 주 목적
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke(ID);
    }

    /// <summary>
    /// 슬롯 위에서 움직임이 있었던 것을 인벤토리UI에 전달하는 것이 주 목적
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerMove(PointerEventData eventData)
    {
        onPointerMove?.Invoke(eventData.position);
    }

    /// <summary>
    /// UI가 갱신될 때 ItemSlotUI가 추가로 해야될 작업 수행하는 함수
    /// </summary>
    protected override void OnRefresh()
    {
        if( ItemSlot.IsEquipped)
        {
            equipped.color = Color.red;     // 장비 중이면 빨간색
        }
        else
        {
            equipped.color = Color.clear;   // 장비 안했으면 투명하게
        }
    }
}
