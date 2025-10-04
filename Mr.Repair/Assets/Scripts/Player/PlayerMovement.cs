using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 150f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        // プレイヤーが倒れないように X,Z軸の回転を固定
    }

    /// <summary>
    /// 移動入力を設定（InputSystemなどから呼び出す）
    /// </summary>
    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    /// <summary>
    /// 視点入力を設定（InputSystemなどから呼び出す）
    /// </summary>
    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleLookHorizontal();
    }

    private void LateUpdate()
    {
        HandleLookVertical();
    }

    // ====================================
    // 移動処理
    // ====================================
    private void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 move = forward * moveInput.y + right * moveInput.x;
        if (move.magnitude > 1f) move.Normalize();

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    // ====================================
    // 視点処理（左右/Yaw）
    // ====================================
    private void HandleLookHorizontal()
    {
        float yaw = lookInput.x * lookSensitivity * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    // ====================================
    // 視点処理（上下/Pitch）
    // ====================================
    private void HandleLookVertical()
    {
        pitch -= lookInput.y * lookSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
