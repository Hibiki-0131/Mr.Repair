using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerLook), typeof(PlayerAction))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerLook look;
    private PlayerAction action;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
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
        look.SetLookInput(context.ReadValue<Vector2>());
    }

    // ƒWƒƒƒ“ƒv“ü—Í
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            action.Jump();
    }
}
