using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Player_Health : Test_Base
{
    Player player;

    private void Start()
    {
        player = GameManager.Inst.Player;
    }

    protected override void Test1(InputAction.CallbackContext _)
    {
        player.HP += 10;
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        player.HP -= 10;
    }

    protected override void Test3(InputAction.CallbackContext _)
    {
        player.MP += 10;
    }

    protected override void Test4(InputAction.CallbackContext _)
    {
        player.MP -= 10;
    }

    protected override void Test5(InputAction.CallbackContext _)
    {
        player.ManaRegenerate(30, 3);
    }
}
