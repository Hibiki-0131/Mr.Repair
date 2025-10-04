using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        // 移動処理（カメラ基準ではなくプレイヤー基準）
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 move = forward * moveInput.y + right * moveInput.x;
        if (move.magnitude > 1f) move.Normalize();

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}

