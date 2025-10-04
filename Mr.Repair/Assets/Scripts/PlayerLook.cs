using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 150f;

    private Vector2 lookInput;
    private float pitch;

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void LateUpdate()
    {
        // 左右（Yaw）→ プレイヤー本体を回転
        float yaw = lookInput.x * lookSensitivity * Time.deltaTime;
        transform.Rotate(0f, yaw, 0f);

        // 上下（Pitch）→ カメラだけを回転
        pitch -= lookInput.y * lookSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}

