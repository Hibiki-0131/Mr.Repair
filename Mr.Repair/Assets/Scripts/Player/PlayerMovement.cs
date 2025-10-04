using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        // 物理で倒れないように
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void FixedUpdate()
    {
        // 移動処理（カメラ基準ではなくプレイヤー基準）
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 move = forward * moveInput.y + right * moveInput.x;
        if (move.magnitude > 1f) move.Normalize();

        // Rigidbody を使って移動
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}
