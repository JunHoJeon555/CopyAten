using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_InventoryUI : Test_Base
{
    public uint invenSize = Inventory.Default_Inventory_Size;
    public InventoryUI inventoryUI;


    public ItemCode code = ItemCode.Ruby;
    public int index = 0;
    public uint from = 0;
    public uint to = 1;

    Inventory inventory;

    private void Start()
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

        // inventoryUI.TestInventory_Spliter(2);
    }

    protected override void Test1(InputAction.CallbackContext _)
    {
        uint temp = (uint)Mathf.Clamp(index, 0, inventory.SlotCount);
        inventory.AddItem(code, temp);
        inventory.PrintInventory();
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        uint temp = (uint)Mathf.Clamp(index, 0, inventory.SlotCount);
        inventory.RemoveItem(temp);
        inventory.PrintInventory();
    }

    protected override void Test3(InputAction.CallbackContext _)
    {
        uint temp = (uint)Mathf.Clamp(index, 0, inventory.SlotCount);
        inventory.ClearSlot(temp);
        inventory.PrintInventory();
    }

    protected override void Test4(InputAction.CallbackContext _)
    {
        inventory.MoveItem(from, to);
        inventory.PrintInventory();
    }

    protected override void Test5(InputAction.CallbackContext _)
    {
        inventory = new Inventory(null, invenSize);
        inventoryUI.InitializeInventory(inventory);
    }
}
