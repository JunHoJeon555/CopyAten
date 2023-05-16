using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 아이템 데이터 매니저
    /// </summary>
    ItemDataManager itemDataManager;
    public ItemDataManager ItemData => itemDataManager;

    /// <summary>
    /// 인벤토리 UI
    /// </summary>
    InventoryUI inventoryUI;
    public InventoryUI InvenUI => inventoryUI;

    /// <summary>
    /// 플레이어
    /// </summary>
    Player player;
    public Player Player => player;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        itemDataManager = GetComponent<ItemDataManager>();
    }

    protected override void Initialize()
    {
        base.Initialize();
        inventoryUI = FindObjectOfType<InventoryUI>();
        player = FindObjectOfType<Player>();
    }
}
