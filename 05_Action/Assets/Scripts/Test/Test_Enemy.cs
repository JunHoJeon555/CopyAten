using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Enemy : Test_Base
{
    Player player;
    Enemy enemy;

    private void Start()
    {
        enemy = FindObjectOfType<Enemy>();
    }

    protected override void Test1(InputAction.CallbackContext _)
    {
        player = GameManager.Inst.Player;
        player.Test_AddItem(ItemCode.SilverSword);
        player.Inventory[0].EquipItem(player.gameObject);
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        enemy.Die();
    }

    protected override void Test3(InputAction.CallbackContext _)
    {
        enemy.Test_ItemDrop();
    }

    protected override void Test4(InputAction.CallbackContext _)
    {
        enemy.Test_DropCheck(1000000);
    }

    protected override void Test5(InputAction.CallbackContext _)
    {
        enemy.Defence(40);
    }
}
