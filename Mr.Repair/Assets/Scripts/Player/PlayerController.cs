using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimation))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAnimation animationControl;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animationControl = GetComponent<PlayerAnimation>();
    }

    // 移動入力
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        movement.SetMoveInput(input);
        animationControl.SetIsWalking(input.sqrMagnitude > 0.01f);
    }

    // 部品化ボタン（RT長押し）
    public void OnParts(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movement.BeginPartsMode();
            animationControl.SetPartsState(true);
            animationControl.PlayTransformAnimation(true);
        }
        else if (context.canceled)
        {
            movement.TryEndPartsMode(); // ← 狭い場所では解除されない
            if (!movement.IsPartsMode)
            {
                animationControl.SetPartsState(false);
                animationControl.PlayTransformAnimation(false);
            }
        }
    }
}
