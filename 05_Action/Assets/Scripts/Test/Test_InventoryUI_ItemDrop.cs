using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_InventoryUI_ItemDrop : Test_Base
{
    public uint invenSize = Inventory.Default_Inventory_Size;
    public InventoryUI inventoryUI;
    Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory(null, invenSize);
        inventoryUI.InitializeInventory(inventory);
        inventory.AddItem(ItemCode.Ruby);
        inventory.AddItem(ItemCode.Ruby);
        inventory.AddItem(ItemCode.Ruby, 1);
        inventory.AddItem(ItemCode.Emerald, 2); // 2번에 8개
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 2);
        inventory.AddItem(ItemCode.Emerald, 3); // 3번에 4개
        inventory.AddItem(ItemCode.Emerald, 3);
        inventory.AddItem(ItemCode.Emerald, 3);
        inventory.AddItem(ItemCode.Emerald, 3);
        inventory.AddItem(ItemCode.Sapphire, 5);
        inventory.AddItem(ItemCode.Sapphire, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
