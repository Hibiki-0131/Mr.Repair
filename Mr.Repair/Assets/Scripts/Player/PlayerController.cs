using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAction))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAction action;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        action = GetComponent<PlayerAction>();
    }

    // 移動入力
    public void OnMove(InputAction.CallbackContext context)
    {
        movement.SetMoveInput(context.ReadValue<Vector2>());
    }

    // 走る入力
    public void OnRun(InputAction.CallbackContext context)
    {
        // RunはPlayerMovement側でスタミナを管理
        if (context.performed || context.canceled)
        {
            movement.SetRunInput(context.ReadValue<float>() > 0.5f);
        }
    }

    // 視点入力
    public void OnLook(InputAction.CallbackContext context)
    {
        movement.SetLookInput(context.ReadValue<Vector2>());
    }

    // ジャンプ入力
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Jump();
    }

    // Interact
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Interact();
    }

    // Use
    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Use();
    }
}
