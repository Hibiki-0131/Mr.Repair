using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    // SendMessages が探すのはこのシグネチャだけ
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log($"[TEST] OnMove invoked! Phase={context.phase}");
        Vector2 input = context.ReadValue<Vector2>();
        Debug.Log($"[TEST] Move Input: {input}");
        movement.SetMoveInput(input);
    }
}
