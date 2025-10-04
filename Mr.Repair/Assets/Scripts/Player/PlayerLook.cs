using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 150f;

    private Rigidbody rb;
    private Vector2 lookInput;
    private float pitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        // プレイヤーが倒れないように Y軸以外の回転を固定
    }

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void FixedUpdate()
    {
        // 左右（Yaw）→ Rigidbodyで回転
        float yaw = lookInput.x * lookSensitivity * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void LateUpdate()
    {
        // 上下（Pitch）→ カメラだけ回転
        pitch -= lookInput.y * lookSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}

