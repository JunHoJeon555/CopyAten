using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    // 필요 상수 -----------------------------------------------------------------------------------

    /// <summary>
    /// 기본 인벤토리 크기
    /// </summary>
    public const int Default_Inventory_Size = 6;

    /// <summary>
    /// 임시 슬롯의 인덱스
    /// </summary>
    public const uint TempSlotIndex = 99999999;

    // 변수 ---------------------------------------------------------------------------------------

    /// <summary>
    /// 이 인벤토리에 들어있는 슬롯의 배열
    /// </summary>
    ItemSlot[] slots;

    /// <summary>
    /// 인벤토리 슬롯에 접근하기 위한 인덱서
    /// </summary>
    /// <param name="index">접근할 슬롯의 인덱스</param>
    /// <returns>접근할 슬롯</returns>
    public ItemSlot this[uint index] => slots[index];

    /// <summary>
    /// 인벤토리 슬롯의 갯수
    /// </summary>
    public int SlotCount => slots.Length;

    /// <summary>
    /// 임시 슬롯(드래그나 분리할 때 사용)
    /// </summary>
    ItemSlot tempSlot;
    public ItemSlot TempSlot => tempSlot;

    /// <summary>
    /// 게임 메니저가 가지고 있는 아이템 데이터 메니저(모든 아이템의 데이터를 가지고 있다.(종류별))
    /// </summary>
    ItemDataManager dataManager;

    /// <summary>
    /// 이 인벤토리를 가지고 있는 플레이어
    /// </summary>
    Player owner;
    public Player Owner => owner;
    

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="size">새로 만들 인벤토리의 크기</param>
    public Inventory(Player owner, uint size = Default_Inventory_Size)
    {
        Debug.Log($"{size}칸짜리 인벤토리 생성");
        slots = new ItemSlot[size];             // 슬롯용 배열 만들기
        for(uint i=0;i<size; i++)
        {
            slots[i] = new ItemSlot(i);         // 슬롯 하나씩 생성
        }
        tempSlot = new ItemSlot(TempSlotIndex); // 임시 슬롯 만들고

        dataManager = GameManager.Inst.ItemData;// 데이터 메니저 캐싱해놓기

        this.owner = owner;
    }

    /// <summary>
    /// 아이템을 1개 추가하는 함수
    /// </summary>
    /// <param name="data">추가될 아이템의 데이터</param>
    /// <returns>성공여부(true면 추가, false 추가실패)</returns>
    public bool AddItem(ItemData data)
    {
        bool result = false;

        // 같은 종류의 아이템이 있는지
        ItemSlot sameDataSlot = FindSameItem(data);
        if(sameDataSlot != null)
        {
            // 같은 종류의 아이템이 있으면 증가 시도
            result = sameDataSlot.IncreaseSlotItem(out uint _); // 넘치는 갯수는 의미없음. 결과만 사용
        }
        else
        {
            // 같은 종류의 아이템이 없으면 빈슬롯 찾기
            ItemSlot emptySlot = FindEmptySlot();
            if (emptySlot != null)
            {
                // 빈슬롯 찾았다.
                emptySlot.AssignSlotItem(data);
                result = true;
            }
            else
            {
                // 비어있는 슬롯이 없다.
                Debug.Log("실패 : 인벤토리가 가득 찼습니다.");
            }
        }
        return result;
    }

    /// <summary>
    /// 아이템을 1개 추가하는 함수
    /// </summary>
    /// <param name="code">추가될 아이템의 enum</param>
    /// <returns>성공여부(true면 추가, false 추가실패)</returns>
    public bool AddItem(ItemCode code)
    {
        return AddItem(dataManager[code]);
    }

    /// <summary>
    /// 아이템을 인벤토리의 특정 슬롯에 1개 추가하는 함수
    /// </summary>
    /// <param name="data">추가할 아이템 데이터</param>
    /// <param name="index">아이템이 추가될 인덱스</param>
    /// <returns>true면 성공, false면 실패</returns>
    public bool AddItem(ItemData data, uint index)
    {
        bool result = false;
                
        if(IsValidIndex(index)) // 적절한 인덱스인지 확인
        {
            ItemSlot slot = slots[index];  // index에 해당하는 슬롯 찾아오기

            if (slot.IsEmpty)
            {
                // 슬롯이 비어있으면 그냥 추가
                slot.AssignSlotItem(data);
            }
            else
            {
                // 슬롯이 비어있지 않다.
                if (slot.ItemData == data)
                {
                    result = slot.IncreaseSlotItem(out uint _); // 같은 아이템이면 증가 시도
                }
                else
                {
                    // 다른 아이템이면 그냥 실패
                    Debug.Log($"실패 : 인벤토리 {index}번 슬롯에 다른 아이템이 들어있습니다.");
                }
            }
        }
        else
        {
            Debug.Log($"실패 : {index}번은 잘못된 인덱스입니다.");
        }

        return result;
    }

    /// <summary>
    /// 아이템을 인벤토리의 특정 슬롯에 1개 추가하는 함수
    /// </summary>
    /// <param name="code">추가할 아이템 enum코드</param>
    /// <param name="index">아이템이 추가될 인덱스</param>
    /// <returns>true면 성공, false면 실패</returns>
    public bool AddItem(ItemCode code, uint index)
    {
        return AddItem(dataManager[code], index);
    }

    /// <summary>
    /// 인벤토리 특정 슬롯에서 일정 갯수만큼 아이템 제거하는 함수
    /// </summary>
    /// <param name="slotIndex">제거할 슬롯 인덱스</param>
    /// <param name="decreaseCount">감소시킬 갯수</param>
    public void RemoveItem(uint slotIndex, uint decreaseCount = 1)
    {
        if(IsValidIndex(slotIndex))
        {
            ItemSlot slot = slots[slotIndex];
            slot.DecreaseSlotItem(decreaseCount);
        }
        else
        {
            Debug.Log($"실패 : {slotIndex}는 잘못된 인덱스입니다.");
        }
    }
    
    
    /// <summary>
    /// 특정 슬롯에서 아이템을 완전히 제거하는 함수
    /// </summary>
    /// <param name="slotIndex">제거할 슬롯 인덱스</param>
    public void ClearSlot(uint slotIndex)
    {
        if(IsValidIndex(slotIndex))
        {
            ItemSlot slot = slots[slotIndex];
            slot.ClearSlotItem();
        }
        else
        {
            Debug.Log($"실패 : {slotIndex}는 잘못된 인덱스입니다.");
        }
    }

    /// <summary>
    /// 인벤토리를 전부 비우는 함수
    /// </summary>
    public void ClearInventory()
    {
        foreach(var slot in slots)
        {
            slot.ClearSlotItem();
        }
    }

    /// <summary>
    /// 아이템을 이동 시키는 함수
    /// </summary>
    /// <param name="from">시작 슬롯의 인덱스</param>
    /// <param name="to">도착 슬롯의 인덱스</param>
    public void MoveItem(uint from, uint to)
    {
        // from과 to가 같은 경우는 스킵
        // from과 to 모두 valid해야 한다.
        if ( (from != to) &&  IsValidIndex(from) && IsValidIndex(to) )    
        {
            // temp슬롯을 감안해서 삼항연산자로 슬롯 구하기.
            ItemSlot fromSlot = (from == TempSlotIndex) ? TempSlot : slots[from];
            if( !fromSlot.IsEmpty ) // from이 빈 경우는 처리 안함(실질적으로 사용안함)
            {
                ItemSlot toSlot = (to == TempSlotIndex) ? TempSlot : slots[to];

                if( fromSlot.ItemData == toSlot.ItemData )  
                {
                    // from과 to가 같은 아이템인 경우. 아이템 합치기
                    toSlot.IncreaseSlotItem(out uint overCount, fromSlot.ItemCount );
                    fromSlot.DecreaseSlotItem(fromSlot.ItemCount - overCount);
                    Debug.Log($"인벤토리의 {from}슬롯에서 {to}슬롯으로 아이템 합치기 성공");
                }
                else
                {
                    // from과 to가 다른 아이템인 경우. 서로 스왑하기
                    ItemData tempData = fromSlot.ItemData;
                    uint tempCount = fromSlot.ItemCount;
                    fromSlot.AssignSlotItem(toSlot.ItemData, toSlot.ItemCount);
                    toSlot.AssignSlotItem(tempData, tempCount);
                    Debug.Log($"인벤토리의 {from}슬롯과 {to}슬롯의 아이템 교체 성공");
                }
            }
        }
    }

    /// <summary>
    /// 특정 슬롯에서 아이템을 분리해 Temp슬롯으로 옮기는 함수
    /// </summary>
    /// <param name="slotID">분리할 슬롯</param>
    /// <param name="count">분리할 갯수</param>
    public void SplitItem(uint slotID, uint count)
    {
        if (IsValidIndex(slotID))
        {
            ItemSlot fromSlot = slots[slotID];
            fromSlot.DecreaseSlotItem(count);
            TempSlot.AssignSlotItem(fromSlot.ItemData, count);
        }
    }

    //아이템 정렬
    /// <summary>
    /// 아이템 정렬용 함수
    /// </summary>
    /// <param name="sortBy">정렬 기준</param>
    /// <param name="isAscending">true면 오름차순, false면 내림차순</param>
    public void SlotSorting(ItemSortBy sortBy, bool isAscending = true)
    {
        // 정렬할 리스트 만들기
        List<ItemSlot> sortSlots = new List<ItemSlot>(SlotCount);
        foreach(var slot in slots )
        {
            sortSlots.Add(slot);
        }

        // 파라메터에서 설정한 기준에 따라 정렬
        switch(sortBy)
        {
            case ItemSortBy.Name:
                sortSlots.Sort((x, y) =>
                {
                    if(x.ItemData == null)  
                    {
                        return 1;       // x가 null이면 x가 크다.
                    }
                    if(y.ItemData == null)
                    {
                        return -1;      // y가 null이면 y가 크다.
                    }
                    if (isAscending)
                    {
                        return x.ItemData.itemName.CompareTo(y.ItemData.itemName);  // 디폴트로 오름차순
                    }
                    else
                    {
                        return y.ItemData.itemName.CompareTo(x.ItemData.itemName);  // 내림차순으로 처리
                    }
                });
                break;
            case ItemSortBy.Price:
                sortSlots.Sort((x, y) =>
                {
                    if (x.ItemData == null)
                    {
                        return 1;
                    }
                    if (y.ItemData == null)
                    {
                        return -1;
                    }
                    if (isAscending)
                    {
                        return x.ItemData.price.CompareTo(y.ItemData.price);
                    }
                    else
                    {
                        return y.ItemData.price.CompareTo(x.ItemData.price);
                    }
                });
                break;
            case ItemSortBy.Code:
            default:
                sortSlots.Sort((x, y) =>
                {
                    if (x.ItemData == null)
                    {
                        return 1;
                    }
                    if (y.ItemData == null)
                    {
                        return -1;
                    }
                    if (isAscending)
                    {
                        return x.ItemData.code.CompareTo(y.ItemData.code);
                    }
                    else
                    {
                        return y.ItemData.code.CompareTo(x.ItemData.code);
                    }
                });
                break;
        }

        // 정렬된 결과에 따라 슬롯 순서 조절하기
        List<(ItemData, uint)> sortedData = new List<(ItemData, uint)>(SlotCount);
        foreach (var slot in sortSlots)
        {
            // 아이템 데이터와 아이템 갯수를 정렬 순서에 맞춰서 리스트에 저장하기
            sortedData.Add((slot.ItemData, slot.ItemCount));    
        }

        int index = 0;
        foreach (var data in sortedData)
        {
            // 저장해 놓은 데이터에 따라 슬롯에 아이템 설정
            slots[index].AssignSlotItem(data.Item1, data.Item2);
            index++;
        }
    }

    // 단순 확인 및 탐색용 함수들 -------------------------------------------------------------------

    private bool IsValidIndex(uint index) => (index < SlotCount) || (index == TempSlotIndex);    

    /// <summary>
    /// 비어있는 슬롯을 찾아주는 함수
    /// </summary>
    /// <returns>null이면 비어있는 함수가 없다. null이 아니면 찾았다.</returns>
    private ItemSlot FindEmptySlot()
    {
        ItemSlot result = null;
        foreach(ItemSlot slot in slots) // 그냥 다 뒤져보기
        {
            if(slot.IsEmpty)
            {
                result = slot;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 인벤토리에 같은 종류의 아이템이 있는지 찾아주는 함수(최대갯수인 슬롯은 제외)
    /// </summary>
    /// <param name="data">찾을 아이템</param>
    /// <returns>같은 종류의 아이템이 들어있는 슬롯(null이 아니면 찾았다. null이면 없다.)</returns>
    private ItemSlot FindSameItem(ItemData data)
    {
        ItemSlot findSlot = null;
        foreach (ItemSlot slot in slots)    // 전부 찾기
        {
            // 같은 종류의 아이템 데이터고 슬롯에 빈용량이 있어야 한다.
            if(slot.ItemData == data && slot.ItemCount < slot.ItemData.maxStackCount)
            {
                findSlot = slot;
                break;
            }
        }
        return findSlot;
    }

    /// <summary>
    /// 테스트 확인용 함수
    /// </summary>
    public void PrintInventory()
    {
        // 출력 예시 : [ 루비(1), 사파이어(1), 에메랄드(2), (빈칸), (빈칸), (빈칸) ]
        string printText = "[ ";

        for(int i =0;i<SlotCount-1;i++)
        {
            if (!slots[i].IsEmpty)
            {
                printText += $"{slots[i].ItemData.itemName}({slots[i].ItemCount})";
            }
            else
            {
                printText += "(빈칸)";
            }
            printText += ", ";
        }

        ItemSlot last = slots[SlotCount - 1];
        if( !last.IsEmpty)
        {
            printText += $"{last.ItemData.itemName}({last.ItemCount})";
        }
        else
        {
            printText += "(빈칸)";
        }
        printText += " ]";

        Debug.Log(printText);
    }
}
