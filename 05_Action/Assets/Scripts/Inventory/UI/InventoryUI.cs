using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// 슬롯을 다시 만들때 필요한 프리팹
    /// </summary>
    public GameObject slotPrefab;

    /// <summary>
    /// 이 UI가 보여줄 인벤토리
    /// </summary>
    Inventory inven;

    public Player Owner => inven.Owner;

    /// <summary>
    /// 이 인벤토리 UI에 있는 모든 슬롯UI
    /// </summary>
    ItemSlotUI[] slotUIs;

    /// <summary>
    /// 아이템 이동이나 분리할 때 사용할 임시 슬롯UI 
    /// </summary>
    TempItemSlotUI tempSlotUI;

    /// <summary>
    /// 아이템의 상세정보를 보여주는 창
    /// </summary>
    DetailWindow detail;

    /// <summary>
    /// 슬롯에 들어있는 아이템을 분리하기 위한 창
    /// </summary>
    ItemSpliterUI spliter;

    /// <summary>
    /// Owner가 가지고 있는 돈을 표시하는 창
    /// </summary>
    MoneyPanel money;

    /// <summary>
    /// 인벤토리 닫기 버튼
    /// </summary>
    Button closeButton;

    /// <summary>
    /// 인벤토리 열고/닫는 효과를 위한 컴포넌트
    /// </summary>
    CanvasGroup canvasGroup;

    /// <summary>
    /// 인벤토리가 열렸을 때 실행될 델리게이트
    /// </summary>
    public Action onInventoryOpen;

    /// <summary>
    /// 인벤토리가 닫혔을 때 실행될 델리게이트
    /// </summary>
    public Action onInventoryClose;

    PlayerInputActions inputActions;
    bool isShiftPress = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Transform slotParent = transform.GetChild(0);
        slotUIs = slotParent.GetComponentsInChildren<ItemSlotUI>();

        Transform child = transform.GetChild(1);
        closeButton = child.GetComponent<Button>();
        closeButton.onClick.AddListener(Close);

        tempSlotUI = GetComponentInChildren<TempItemSlotUI>();

        detail = GetComponentInChildren<DetailWindow>();

        spliter = GetComponentInChildren<ItemSpliterUI>();
        spliter.onOKClick += OnSplitOK;

        money = GetComponentInChildren<MoneyPanel>();

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Shift.performed += OnShiftPress;
        inputActions.UI.Shift.canceled += OnShiftPress;
        inputActions.UI.Click.canceled += OnItemDrop;
        inputActions.UI.InvenforyOnOff.performed += OnInventoryShortCut;
    }

    private void OnDisable()
    {
        inputActions.UI.InvenforyOnOff.performed -= OnInventoryShortCut;
        inputActions.UI.Click.canceled -= OnItemDrop;
        inputActions.UI.Shift.canceled -= OnShiftPress;
        inputActions.UI.Shift.performed -= OnShiftPress;
        inputActions.UI.Disable();
    }

    /// <summary>
    /// 인벤토리 UI 초기화
    /// </summary>
    /// <param name="playerInven">이 UI가 표시할 인벤토리</param>
    public void InitializeInventory(Inventory playerInven)
    {
        inven = playerInven;

        Transform slotParent = transform.GetChild(0);
        GridLayoutGroup layout = slotParent.GetComponent<GridLayoutGroup>();

        if( Inventory.Default_Inventory_Size != inven.SlotCount )   
        {
            // 기본 크기와 달라졌을 때

            // 미리 만들어져 있던 슬롯 모두 삭제
            foreach ( var slot in slotUIs )
            {
                Destroy( slot.gameObject );     
            }

            // 셀 크기 계산하기
            RectTransform rect = (RectTransform)slotParent;
            float totalArea = rect.rect.height * rect.rect.width;   // 부모 영역의 전체 넓이
            float slotArea = totalArea / inven.SlotCount;           // 슬롯 하나가 가질 수 있는 넓이
            float slotSideLength = Mathf.Floor( Mathf.Sqrt( slotArea )); // 슬롯 한변의 길이 구하기
            layout.cellSize = new Vector2(slotSideLength, slotSideLength ); // 구한 길이로 적용하기

            // 슬롯 새로 만들기
            slotUIs = new ItemSlotUI[inven.SlotCount];
            for (uint i = 0; i < inven.SlotCount; i++)
            {
                GameObject obj = Instantiate(slotPrefab, slotParent);   // 생성하고
                obj.name = $"{slotPrefab.name}_{i}";                    // 이름 붙이고
                slotUIs[i] = obj.GetComponent<ItemSlotUI>();            // 저장해 놓기
            }
        }

        // 슬롯의 초기화 작업
        for(uint i=0;i<inven.SlotCount;i++)
        {
            slotUIs[i].InitializeSlot(i, inven[i]);
            slotUIs[i].onDragBegin += OnItemMoveBegin;
            slotUIs[i].onDragEnd += OnItemMoveEnd;
            slotUIs[i].onClick += OnSlotClick;
            slotUIs[i].onPointerEnter += OnItemDetailOn;
            slotUIs[i].onPointerExit += OnItemDetailOff;
            slotUIs[i].onPointerMove += OnSlotPointerMove;
        }
        // 임시 슬롯 초기화
        tempSlotUI.InitializeSlot(Inventory.TempSlotIndex, inven.TempSlot); // 임시슬롯도 초기화
        tempSlotUI.onTempSlotOpenClose += OnDetailPause;
        tempSlotUI.Close();     // 시작하면 꺼놓기

        // 상세정보창 닫아 놓기
        detail.Close();

        // 분리창 닫기
        spliter.Close();

        // 오너의 돈이 변경될 때 머니 패널의 Refresh함수 실행하도록 함수 등록
        Owner.onMoneyChange += money.Refresh;
        money.Refresh(Owner.Money);     // 첫번째는 강제 리프레시

        // 시작할 때 닫아놓기
        Close();
    }

    /// <summary>
    /// 마우스 드래그가 시작되었을 때 실행되는 함수
    /// </summary>
    /// <param name="slotID">드래그 시작 슬롯의 ID</param>
    private void OnItemMoveBegin(uint slotID)
    {
        if (inven[slotID].IsEquipped)   // 드래그 시작할 때 장비되어있으면 장비 해제
        {
            inven[slotID].EquipItem(Owner.gameObject);
        }
        inven.MoveItem(slotID, tempSlotUI.ID);  // 시작 슬롯의 내용과 임시 슬롯의 내용을 서로 교체시키기
        tempSlotUI.Open();                      // 임시 슬롯 보이게 만들기
    }

    /// <summary>
    /// 마우스 드래그가 끝났을 때 실행되는 함수
    /// </summary>
    /// <param name="slotID">드래그가 끝난 슬롯의 ID</param>
    /// <param name="isSuccess">드래그가 성공적으로 끝났으면 true, 취소되었으면 false</param>
    private void OnItemMoveEnd(uint slotID, bool isSuccess)
    {
        inven.MoveItem(tempSlotUI.ID, slotID);  // 임시 슬롯의 내용과 드래그가 끝난 슬롯의 내용을 서로 교체시키기
        if(tempSlotUI.ItemSlot.IsEmpty)         // 교체 결과 임시 슬롯이 비게 되면
        {
            tempSlotUI.Close();                 // 임시 슬롯 비활성화해서 안보이게 만들기
        }

        if(isSuccess)
        {
            detail.Open(inven[slotID].ItemData);// 드래그가 성공적으로 끝났으면 상세정보창 보여주기
        }
    }

    /// <summary>
    /// 슬롯을 클릭했을 때 실행되는 함수
    /// </summary>
    /// <param name="slotID">클릭된 슬롯의 ID</param>
    private void OnSlotClick(uint slotID)
    {
        if( !tempSlotUI.ItemSlot.IsEmpty )      
        {
            // 임시 슬롯이 비어있지 않을 때(=드래그 끝났을 때 다른 아이템이 있어서 교체 후 들고 있는 상황)
            // 이때는 드래그가 끝났을 때와 똑같이 처리한다.

            OnItemMoveEnd(slotID, true);    // 임시슬롯과 클릭된 슬롯의 내용을 서로 교체
        }
        else
        {
            // 임시 슬롯이 비어있는데 클릭했다.
            if( isShiftPress)
            {
                // 아이템 분리 용도로 클릭했다.
                OnSplitOpen(slotID);                
            }
            else
            {
                // 아이템 사용 또는 장비하기 위해 클릭했다.

                inven[slotID].UseItem(Owner.gameObject);    // 소유자에게 아이템 사용하기
                inven[slotID].EquipItem(Owner.gameObject);  // 소유자가 아이템 장비/해제해보기
            }
        }
    }

    /// <summary>
    /// 마우스 포인터가 슬롯에 들어갔을 때 그 슬롯의 아이템 정보를 상세히 보여주는 창 여는 함수
    /// </summary>
    /// <param name="slotID"></param>
    private void OnItemDetailOn(uint slotID)
    {
        detail.Open(slotUIs[slotID].ItemSlot.ItemData);
    }

    /// <summary>
    /// 마우스 포인터가 슬롯에서 나갔을 때 아이템 상세정보창을 닫는 함수
    /// </summary>
    /// <param name="slotID"></param>
    private void OnItemDetailOff(uint slotID)
    {
        detail.Close();
    }

    /// <summary>
    /// 마우스가 슬롯안에서 움직일 때 실행되는 함수
    /// </summary>
    /// <param name="screenPos">마우스 커서의 스크린 좌표</param>
    private void OnSlotPointerMove(Vector2 screenPos)
    {
        detail.MovePosition(screenPos); // 디테일 창 이동시키기
    }

    /// <summary>
    /// 디테일창이 일시 정지 될지 말지를 결정하는 함수(주로 임시슬롯이 열릴때 일시정지함)
    /// </summary>
    /// <param name="isPause">true면 일시정지, false면 일시정지 해제</param>
    private void OnDetailPause(bool isPause)
    {
        detail.IsPause = isPause;
    }

    /// <summary>
    /// 아이템 분리창 여는 함수
    /// </summary>
    /// <param name="slotID">아이템을 분리할 슬롯의 ID</param>
    private void OnSplitOpen(uint slotID)
    {
        ItemSlotUI targetSlotUI = slotUIs[slotID];  // transform이 필요해서 ItemSlotUI 가져옴
        spliter.transform.position = targetSlotUI.transform.position + Vector3.up * 100;    // 슬롯의 위쪽에 배치
        spliter.Open(targetSlotUI.ItemSlot);        // 분리창 열기
        detail.IsPause = true;                      // 디테일창 일시 정지 시키기.(내부에서 알아서 닫음)
    }

    /// <summary>
    /// 아이템 분리창에서 OK버튼이 눌러졌을 때 실행되는 함수
    /// </summary>
    /// <param name="slotID">아이템을 분리할 슬롯의 아이디</param>
    /// <param name="count">덜어낼 아이템 갯수</param>
    private void OnSplitOK(uint slotID, uint count)
    {
        inven.SplitItem(slotID, count);
        tempSlotUI.Open();
    }

    /// <summary>
    /// 쉬프트키를 눌렀을 때 실행되는 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnShiftPress(InputAction.CallbackContext context)
    {
        isShiftPress = !context.canceled;
    }


    /// <summary>
    /// 임시 슬롯이 들고 있는 아이템을 드랍 시도하는 함수
    /// </summary>
    /// <param name="_"></param>
    private void OnItemDrop(InputAction.CallbackContext _)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue(); // 마우스 커서 위치 가져오기
        if (!IsInInventoryArea(screenPos))
        {
            // 인벤토리 영역 밖이면
            tempSlotUI.OnDrop(screenPos);   // 아이템 드랍 시도
        }
    }

    /// <summary>
    /// screenPos가 인벤토리UI 안인지 밖인지 확인하는 함수
    /// </summary>
    /// <param name="screenPos">확인할 스크린 좌표</param>
    /// <returns>true면 인벤토리 안쪽, false면 인벤토리 밖</returns>
    private bool IsInInventoryArea(Vector2 screenPos)
    {
        RectTransform rect = (RectTransform)transform;
        Vector2 min = new(rect.position.x - rect.sizeDelta.x, rect.position.y); // 사각형의 왼쪽 아래
        Vector2 max = new(rect.position.x, rect.position.y + rect.sizeDelta.y); // 사각형의 오른쪽 위

        return min.x < screenPos.x && screenPos.x < max.x && min.y < screenPos.y && screenPos.y < max.y;
    }

    /// <summary>
    /// 인벤토리가 열리고 닫히는 입력(I키)이 들어오면 실행될 함수
    /// </summary>
    /// <param name="_"></param>
    private void OnInventoryShortCut(InputAction.CallbackContext _)
    {
        if(canvasGroup.interactable)
        {
            // 열려있으면 닫고
            Close();
        }
        else
        {
            // 닫혀있으면 열기
            Open();

        }
    }

    private void Open()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        onInventoryOpen?.Invoke();
    }

    private void Close()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        onInventoryClose?.Invoke();
    }

    /// <summary>
    /// 테스트 용도
    /// </summary>
    /// <param name="id"></param>
    public void TestInventory_Spliter(uint id)
    {
        spliter.Open(inven[id]);
    }
}
