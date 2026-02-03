using UnityEngine;

// 1. BoxColliderからCapsuleColliderに変更
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float partsSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Normal Capsule")]
    [SerializeField] private Vector3 normalCenter = new(0.02037199f, -0.09777594f, 0.07615082f);
    [SerializeField] private float normalHeight = 0.8783869f;
    [SerializeField] private float normalRadius = 0.2765165f;

    [Header("Parts Capsule (Crouch)")]
    [SerializeField] private Vector3 partsCenter = new(0.02037199f, -0.2604529f, 0.07615082f);
    [SerializeField] private float partsHeight = 0.5530331f;
    [SerializeField] private float partsRadius = 0.2765165f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 3f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider; // 2. 変数を宣言
    private Vector2 moveInput;
    private bool isPartsMode = false;
    private bool isInNarrowSpace = false;

    public bool IsPartsMode => isPartsMode;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>(); // 3. 取得
        
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // 初期サイズの適用
        ApplyColliderSettings(false);
    }

    private void LateUpdate()
    {
        if (CameraManager.Instance != null)
        {
            Camera activeCam = CameraManager.Instance.GetActiveCamera();
            if (activeCam != null && cameraTransform != activeCam.transform)
                cameraTransform = activeCam.transform;
        }

        // 毎フレームColliderの状態を反映
        ApplyColliderSettings(isPartsMode);
    }

    // Colliderの数値を適用する処理を共通化
    private void ApplyColliderSettings(bool toParts)
    {
        if (toParts)
        {
            capsuleCollider.center = partsCenter;
            capsuleCollider.height = partsHeight;
            capsuleCollider.radius = partsRadius;
        }
        else
        {
            capsuleCollider.center = normalCenter;
            capsuleCollider.height = normalHeight;
            capsuleCollider.radius = normalRadius;
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void BeginPartsMode()
    {
        if (!isPartsMode)
        {
            isPartsMode = true;
        }
    }

    public void TryEndPartsMode()
    {
        if (!isInNarrowSpace)
        {
            isPartsMode = false;
        }
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
        HandleMovement();
    }

    private void ApplyCustomGravity()
    {
        Vector3 customGravity = Physics.gravity * gravityMultiplier;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void HandleMovement()
    {
        if (cameraTransform == null)
            return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 100 * Time.fixedDeltaTime);
        }

        float speed = isPartsMode ? partsSpeed : normalSpeed;
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NarrowSpace"))
            isInNarrowSpace = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NarrowSpace"))
            isInNarrowSpace = false;
    }
}