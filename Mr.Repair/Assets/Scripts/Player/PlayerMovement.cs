using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float lookXLimit = 80f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotationX; // pitch
    private bool canLook = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody設定
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // PlayerController から呼ばれる
    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetLookInput(Vector2 input)
    {
        // デバイスノイズ防止
        if (input.sqrMagnitude < 0.0001f)
            input = Vector2.zero;

        lookInput = input;
    }

    private void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 move = (forward * moveInput.y + right * moveInput.x).normalized;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleLook()
    {
        if (!canLook) return;

        // --- 水平回転（プレイヤー本体） ---
        float yaw = lookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up * yaw);

        // --- 垂直回転（カメラ） ---
        rotationX -= lookInput.y * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
