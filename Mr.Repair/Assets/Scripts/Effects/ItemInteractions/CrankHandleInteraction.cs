using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrankHandleInteraction : MonoBehaviour
{
    [Header("回転速度")]
    public float rotateSpeed = 120f;

    [Header("操作対象シャッター")]
    public List<Transform> shutters; // Inspectorでシャッターオブジェクトを登録

    [Header("シャッター下降速度")]
    public float shutterDownSpeed = 2f;

    [Header("シャッターの最終Y位置")]
    public float targetY = 0f;

    private bool isPressed;
    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        interactAction = playerInput.actions["Interact"];

        interactAction.started += ctx =>
        {
            isPressed = true;
            StartCoroutine(RotateAndMoveRoutine());
        };

        interactAction.canceled += ctx =>
        {
            isPressed = false;
        };
    }

    private IEnumerator RotateAndMoveRoutine()
    {
        while (isPressed)
        {
            // ハンドルを回す
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);

            // シャッターを下げる
            foreach (var shutter in shutters)
            {
                if (shutter == null) continue;

                Vector3 pos = shutter.position;
                float nextY = pos.y - shutterDownSpeed * Time.deltaTime;

                if (nextY < targetY) nextY = targetY;
                shutter.position = new Vector3(pos.x, nextY, pos.z);
            }

            yield return null;
        }
    }
}
