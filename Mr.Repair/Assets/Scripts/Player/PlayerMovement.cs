using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float partsSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Collider Size")]
    [SerializeField] private Vector3 normalColliderSize = new Vector3(0.51f, 0.82f, 0.52f);
    [SerializeField] private Vector3 partsColliderSize = new Vector3(0.51f, 0.43f, 0.52f);
    [SerializeField] private Vector3 normalColliderCenter = new Vector3(0.004f, -0.11f, 0.2f);
    [SerializeField] private Vector3 partsColliderCenter = new Vector3(0.004f, -0.3045f, 0.2f);

    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 3f; // ← これを追加（標準の重力の何倍か）

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Vector2 moveInput;
    private bool isPartsMode = false;
    private bool isInNarrowSpace = false;

    public bool IsPartsMode => isPartsMode;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        rb.useGravity = false; // ← Unity標準重力を切る（自前で制御する）
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        boxCollider.size = normalColliderSize;
        boxCollider.center = normalColliderCenter;
    }

    private void LateUpdate()
    {
        // カメラ追従・Collider維持処理（省略同様）
        if (CameraManager.Instance != null)
        {
            Camera activeCam = CameraManager.Instance.GetActiveCamera();
            if (activeCam != null && cameraTransform != activeCam.transform)
                cameraTransform = activeCam.transform;
        }

        if (isPartsMode)
        {
            boxCollider.size = partsColliderSize;
            boxCollider.center = partsColliderCenter;
        }
        else
        {
            boxCollider.size = normalColliderSize;
            boxCollider.center = normalColliderCenter;
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
            UpdateColliderSize(true);
        }
    }

    public void TryEndPartsMode()
    {
        if (!isInNarrowSpace)
        {
            isPartsMode = false;
            UpdateColliderSize(false);
        }
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();  // ← 重力を追加
        HandleMovement();
    }

    /// <summary>
    /// 通常より強い重力を加える
    /// </summary>
    private void ApplyCustomGravity()
    {
        // Unity標準のPhysics.gravityを拡張して適用
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

    private void UpdateColliderSize(bool toParts)
    {
        if (toParts)
        {
            boxCollider.size = partsColliderSize;
            boxCollider.center = partsColliderCenter;
        }
        else
        {
            boxCollider.size = normalColliderSize;
            boxCollider.center = normalColliderCenter;
        }
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
