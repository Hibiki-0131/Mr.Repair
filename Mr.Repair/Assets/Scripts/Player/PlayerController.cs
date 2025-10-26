using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimation))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAnimation anim;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<PlayerAnimation>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        movement.SetMoveInput(input);
        anim.SetIsWalking(input.sqrMagnitude > 0.01f);
    }

    public void OnParts(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movement.BeginPartsMode();
            anim.PlayTransformAnimation(true);
        }
        else if (context.canceled)
        {
            movement.TryEndPartsMode();
            anim.PlayTransformAnimation(false);
        }
    }
}
