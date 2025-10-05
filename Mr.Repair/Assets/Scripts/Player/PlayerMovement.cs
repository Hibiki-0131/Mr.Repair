using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float lookXLimit = 80f;

    [Header("Step Climb Settings")]
    [SerializeField] private float stepHeight = 0.3f;    // 登れる段差
    [SerializeField] private float stepSmooth = 5f;      // Y補正の速さ
    [SerializeField] private float sphereRadius = 0.2f;  // 前方障害物検知

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -20f;       // 手動重力
    [SerializeField] private float maxFallSpeed = -50f;  // 最大落下速度

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotationX;
    private bool canLook = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // Rigidbody設定
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetMoveInput(Vector2 input) => moveInput = input;

    public void SetLookInput(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f) input = Vector2.zero;
        lookInput = input;
    }

    private void Update() => HandleLook();

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyGravity();
        StepClimb();
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 desiredMove = (forward * moveInput.y + right * moveInput.x).normalized;
        Vector3 velocity = desiredMove * moveSpeed;

        // Y速度は重力・StepClimbで制御
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }

    private void ApplyGravity()
    {
        // StepClimb中は上昇補正にY速度を使うため除外
        if (!IsStepClimbing())
        {
            Vector3 v = rb.velocity;
            v.y += gravity * Time.fixedDeltaTime;
            if (v.y < maxFallSpeed) v.y = maxFallSpeed;
            rb.velocity = v;
        }
    }

    private void HandleLook()
    {
        if (!canLook || lookInput.sqrMagnitude < 0.0001f) return;

        // 水平回転
        if (lookInput.x != 0f)
            transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        // 垂直回転
        if (lookInput.y != 0f)
        {
            rotationX -= lookInput.y * lookSensitivity;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    private void StepClimb()
    {
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, sphereRadius, direction, out RaycastHit hit, 0.5f))
        {
            float stepDiff = hit.point.y - transform.position.y;

            if (stepDiff > 0f && stepDiff <= stepHeight)
            {
                Vector3 v = rb.velocity;
                v.y = stepDiff * stepSmooth;
                rb.velocity = v;
            }
        }
    }

    private bool IsStepClimbing()
    {
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, sphereRadius, direction, out RaycastHit hit, 0.5f))
        {
            float stepDiff = hit.point.y - transform.position.y;
            if (stepDiff > 0f && stepDiff <= stepHeight)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        Gizmos.DrawLine(origin, origin + transform.forward * 0.5f);
        Gizmos.DrawWireSphere(origin + transform.forward * 0.5f, sphereRadius);
    }
}
