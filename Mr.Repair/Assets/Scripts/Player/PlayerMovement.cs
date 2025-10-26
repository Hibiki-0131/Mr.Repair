using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform; // 定点カメラを割り当てる

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // 入力がない場合は停止
        if (moveInput.sqrMagnitude < 0.01f)
        {
            isMoving = false;
            rb.velocity = Vector3.zero;
            return;
        }

        isMoving = true;

        // カメラの向きを基準にした前後左右
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // カメラ基準の移動方向を計算
        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // プレイヤーの向きを移動方向に合わせる（ラジコン式）
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }

        // 移動
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }
}
