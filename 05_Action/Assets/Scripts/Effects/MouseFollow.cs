using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Effect.Enable();
        inputActions.Effect.CursorMove.performed += OnMouseMove;
    }

    private void OnDisable()
    {
        inputActions.Effect.CursorMove.performed -= OnMouseMove;
        inputActions.Effect.Disable();
    }

    private void OnMouseMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector3 mousePos = context.ReadValue<Vector2>();
        mousePos.z = 10.0f;
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = target;

        //Debug.Log($"MousePos : {mousePos}");
        //Debug.Log($"Target : {target}");
    }
}
