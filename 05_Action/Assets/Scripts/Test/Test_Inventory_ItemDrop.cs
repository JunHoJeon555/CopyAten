using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Inventory_ItemDrop : Test_Base
{
    public uint count = 3;

    protected override void Test1(InputAction.CallbackContext _)
    {
        ItemFactory.MakeItem(ItemCode.Ruby, count, Vector3.right * 1, true);
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        ItemFactory.MakeItem(ItemCode.Emerald, count, Vector3.right * 2, true);
    }

    protected override void Test3(InputAction.CallbackContext _)
    {
        ItemFactory.MakeItem(ItemCode.Sapphire, count, Vector3.right * 3, true);
    }
}
