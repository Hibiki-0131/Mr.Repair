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

    /*// 部品モード切替（RTボタン）
    public void OnParts(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool toParts = !movement.IsPartsMode;
            movement.SetPartsMode(toParts);
            animationControl.SetPartsState(toParts);
            animationControl.PlayTransformAnimation(toParts);
        }
    }
    */
}
