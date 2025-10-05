using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float lookXLimit = 80f;

    [Header("Step Climb Settings")]
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 5f;
    [SerializeField] private float sphereRadius = 0.2f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float maxFallSpeed = -50f;

    [Header("Animation Settings")]
    [Range(0, 1f)] public float StartAnimTime = 0.3f;
    [Range(0, 1f)] public float StopAnimTime = 0.15f;
    [Range(0, 1f)] public float allowPlayerRotation = 0.1f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 5f;       // 最大スタミナ（秒）
    [SerializeField] private float staminaDrain = 1f;     // 走行中消費速度
    [SerializeField] private float staminaRecover = 0.5f; // 回復速度
    [SerializeField] private float staminaRecoverDelay = 3f; // スタミナ底後の回復待ち時間
    [SerializeField] private PlayerStaminaBar staminaBar; // UI参照

    private float stamina;
    private bool isRunning = false;
    private bool isStaminaEmpty = false;
    private float emptyTimer = 0f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Animator anim;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotationX;
    private bool canLook = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();

        stamina = maxStamina;

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

    public void SetRunInput(bool running)
    {
        // スタミナがあるときだけ走れる
        isRunning = running && stamina > 0f;
    }

    private void Update()
{
    HandleLook();
    UpdateAnimation();
    HandleStamina(); // ← ここで呼ぶ
}


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
        float currentSpeed = moveSpeed;

        if (isRunning)
            currentSpeed *= 2f; // 走る速度に調整（任意）

        Vector3 velocity = desiredMove * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void ApplyGravity()
    {
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

        if (lookInput.x != 0f)
            transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        if (lookInput.y != 0f)
        {
            rotationX -= lookInput.y * lookSensitivity;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    private void HandleStamina()
    {
        if (isRunning && new Vector2(moveInput.x, moveInput.y).sqrMagnitude > 0.01f)
        {
            stamina -= staminaDrain * Time.deltaTime;
            if (stamina <= 0f)
            {
                stamina = 0f;
                isRunning = false;

                // 底になったフラグとタイマー開始
                isStaminaEmpty = true;
                emptyTimer = 0f;
            }
        }
        else
        {
            // スタミナが底かどうかチェック
            if (isStaminaEmpty)
            {
                emptyTimer += Time.deltaTime;
                if (emptyTimer >= staminaRecoverDelay)
                {
                    isStaminaEmpty = false; // 遅延後に回復開始
                }
            }

            if (!isStaminaEmpty)
            {
                stamina += staminaRecover * Time.deltaTime;
                if (stamina > maxStamina) stamina = maxStamina;
            }
        }

        // デバッグ用ログ
        Debug.Log($"Stamina: {stamina:F2}, isRunning: {isRunning}, EmptyDelay: {emptyTimer:F2}");

        // UI更新
        if (staminaBar != null)
            staminaBar.UpdateStamina(stamina, maxStamina);
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
            return stepDiff > 0f && stepDiff <= stepHeight;
        }
        return false;
    }

    private void UpdateAnimation()
    {
        float speed = new Vector2(moveInput.x, moveInput.y).sqrMagnitude;

        if (speed > allowPlayerRotation)
            anim.SetFloat("Blend", speed, StartAnimTime, Time.deltaTime);
        else
            anim.SetFloat("Blend", speed, StopAnimTime, Time.deltaTime);
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
