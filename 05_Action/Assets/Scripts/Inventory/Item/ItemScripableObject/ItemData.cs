using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("아이템 기본 데이터")]
    public ItemCode code;               // 아이템 ID
    public string itemName = "아이템";  // 아이템의 이름
    public GameObject modelPrefab;      // 아이템 외형 프리팹
    public Sprite itemIcon;             // 인벤토리에서 보일 아이콘
    public uint price = 0;              // 아이템의 가격
    public uint maxStackCount = 1;      // 아이템 슬롯 한칸에 몇개까지 들어갈 수 있는지
    public string itemDescription = "설명";   // 아이템 상세 설명
}
