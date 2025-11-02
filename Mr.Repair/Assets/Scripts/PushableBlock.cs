using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [Header("押す設定")]
    [SerializeField] private float pushForce = 6f;         // 押されたときの力
    [SerializeField] private float maxSpeed = 2f;          // 最大速度
    [SerializeField] private float dragWhilePushed = 6f;   // 押され中の抵抗
    [SerializeField] private float dragIdle = 10f;         // 静止中の抵抗
    [SerializeField] private float friction = 0.1f;        // 摩擦を減らす値
    [SerializeField] private string followerTag = "FollowerBlock"; // FollowerBlockのタグ

    [Header("重力設定")]
    [SerializeField] private float gravityMultiplier = 3f; // ← Playerと同じ。標準重力の何倍にするか

    [Header("落下判定設定")]
    [SerializeField] private float fallCheckDistance = 0.6f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector3 lastFollowerPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 20f;
        rb.useGravity = false; // ← Unity標準重力を切る（自前で制御）
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // 滑りやすくする（PhysicMaterial設定）
        Collider col = GetComponent<Collider>();
        PhysicMaterial mat = new PhysicMaterial();
        mat.dynamicFriction = friction;
        mat.staticFriction = friction;
        mat.bounciness = friction;
        mat.frictionCombine = PhysicMaterialCombine.Minimum;
        mat.bounceCombine = PhysicMaterialCombine.Maximum;
        col.material = mat;
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();

        // 地面チェック
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, fallCheckDistance);

        // 空気抵抗を調整
        rb.drag = rb.velocity.magnitude > 0.05f ? dragWhilePushed : dragIdle;

        // 水平方向の速度制限
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(flatVel.x, rb.velocity.y, flatVel.z);
        }
    }

    /// <summary>
    /// PlayerMovementと同様のカスタム重力適用
    /// </summary>
    private void ApplyCustomGravity()
    {
        Vector3 customGravity = Physics.gravity * gravityMultiplier;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void OnCollisionStay(Collision collision)
    {
        // --- プレイヤーの押し ---
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = (transform.position - collision.transform.position);
            pushDir.y = 0f;
            pushDir.Normalize();
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);
        }

        // --- FollowerBlockによる押し（isKinematic対応） ---
        else if (collision.gameObject.CompareTag(followerTag))
        {
            Rigidbody followerRb = collision.rigidbody;
            if (followerRb != null && followerRb.isKinematic)
            {
                Vector3 moveDir = (collision.transform.position - lastFollowerPos);
                moveDir.y = 0f;

                if (moveDir.sqrMagnitude > 0.0001f)
                {
                    moveDir.Normalize();
                    rb.AddForce(moveDir * pushForce, ForceMode.VelocityChange);
                }

                lastFollowerPos = collision.transform.position;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(followerTag))
        {
            lastFollowerPos = Vector3.zero;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.05f,
                        transform.position + Vector3.up * 0.05f + Vector3.down * fallCheckDistance);
    }
#endif
}
