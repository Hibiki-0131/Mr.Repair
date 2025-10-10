using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrankHandleInteraction : MonoBehaviour
{
    [Header("回転速度")]
    public float rotateSpeed = 120f;

    [Header("操作対象シャッター")]
    public List<Transform> shutters;

    [Header("シャッター上昇速度")]
    public float shutterSpeed = 2f;

    [Header("シャッターの最終Y位置")]
    public float targetY = 5f;

    private bool isPressed;
    private PlayerInput playerInput;
    private InputAction interactAction;

    private List<float> initialYPositions = new List<float>();

    private void Start()
    {
        // 各シャッターの初期Y位置を記録
        initialYPositions.Clear();
        foreach (var shutter in shutters)
        {
            if (shutter != null)
                initialYPositions.Add(shutter.position.y);
        }
    }

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
            // ハンドル回転
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);

            // シャッター上昇
            for (int i = 0; i < shutters.Count; i++)
            {
                var shutter = shutters[i];
                if (shutter == null) continue;

                // 現在位置からtargetYまで少しずつ移動
                float currentY = shutter.position.y;
                float newY = Mathf.MoveTowards(currentY, targetY, shutterSpeed * Time.deltaTime);
                shutter.position = new Vector3(shutter.position.x, newY, shutter.position.z);
            }

            yield return null;
        }
    }
}
