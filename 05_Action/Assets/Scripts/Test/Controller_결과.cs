//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class Controller_결과 : MonoBehaviour
//{
//    public float moveSpeed = 5.0f;
//    public float turnSpeed = 180.0f;

//    PlayerInputActions_230217_고병조 inputActions;
//    Vector2 input;

//    private void Awake()
//    {
//        inputActions = new PlayerInputActions_230217_고병조();
//    }

//    private void OnEnable()
//    {
//        inputActions.Player.Enable();
//        inputActions.Player.Move.performed += OnPlayerMove;
//        inputActions.Player.Move.canceled += OnPlayerMove;
//    }

//    private void OnDisable()
//    {
//        inputActions.Player.Move.canceled -= OnPlayerMove;
//        inputActions.Player.Move.performed -= OnPlayerMove;
//        inputActions.Player.Disable();
//    }

//    private void OnPlayerMove(InputAction.CallbackContext context)
//    {
//        input = context.ReadValue<Vector2>();
//    }

//    private void Update()
//    {
//        transform.Translate(input.y * Time.deltaTime * moveSpeed * transform.forward, Space.World);
//        //transform.Rotate(input.x * Time.deltaTime * turnSpeed * Vector3.up);
//        transform.Rotate(0, input.x * Time.deltaTime * turnSpeed, 0);
//    }

//}
