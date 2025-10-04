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

    // ˆÚ“®“ü—Í
    public void OnMove(InputAction.CallbackContext context)
    {
        movement.SetMoveInput(context.ReadValue<Vector2>());
    }

    // Ž‹“_“ü—Í
    public void OnLook(InputAction.CallbackContext context)
    {
        movement.SetLookInput(context.ReadValue<Vector2>());
    }

    // ƒWƒƒƒ“ƒv“ü—Í
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Jump();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Interact();
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Use();
    }
}
