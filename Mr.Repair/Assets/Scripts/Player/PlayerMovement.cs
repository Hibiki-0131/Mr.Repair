using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float partsSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isPartsMode = false;

    public bool IsPartsMode => isPartsMode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        // ゲーム開始時にMainCameraを参照
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        // 毎フレーム、CameraManagerの現在アクティブなカメラを追従
        if (CameraManager.Instance != null)
        {
            Camera activeCam = CameraManager.Instance.GetActiveCamera();
            if (activeCam != null && cameraTransform != activeCam.transform)
            {
                cameraTransform = activeCam.transform;
            }
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetPartsMode(bool active)
    {
        isPartsMode = active;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (cameraTransform == null)
            return;

        if (moveInput.sqrMagnitude < 0.01f)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // カメラ基準方向を取得
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // カメラから見た移動方向
        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // 向きを更新
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 100 * Time.fixedDeltaTime);
        }

        // モードごとの速度設定
        float speed = isPartsMode ? partsSpeed : normalSpeed;
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }
}
