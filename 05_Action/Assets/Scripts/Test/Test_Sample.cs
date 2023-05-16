using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Sample : Test_Base
{
    protected override void Test1(InputAction.CallbackContext _)
    {
        Debug.Log("Press 1");
    }
}
