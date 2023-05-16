using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Player_Attack : Test_Base
{
    Player player;

    void Start()
    {
        player = GameManager.Inst.Player;

    }

    protected override void Test1(InputAction.CallbackContext _)
    {
        player.Test_AddItem(ItemCode.OldSword);
        player.Test_AddItem(ItemCode.IronSword);
        player.Test_AddItem(ItemCode.SilverSword);
        player.EquipItem(EquipType.Weapon, player.Inventory[0]);
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        Time.timeScale = 0.1f;
    }

    protected override void Test3(InputAction.CallbackContext _)
    {
        Time.timeScale = 1.0f;
    }
}
