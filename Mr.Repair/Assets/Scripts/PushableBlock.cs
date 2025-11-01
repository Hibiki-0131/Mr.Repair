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

    [Header("落下設定")]
    [SerializeField] private float fallCheckDistance = 0.6f;
    [SerializeField] private float gravityBoost = 10f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector3 lastFollowerPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // 滑りやすくする
        Collider col = GetComponent<Collider>();
        PhysicMaterial mat = new PhysicMaterial();
        mat.dynamicFriction = friction;
        mat.staticFriction = friction;
        mat.frictionCombine = PhysicMaterialCombine.Minimum;
        col.material = mat;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, fallCheckDistance);

        rb.drag = rb.velocity.magnitude > 0.05f ? dragWhilePushed : dragIdle;

        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityBoost, ForceMode.Acceleration);
        }

        // 水平方向の速度制限
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(flatVel.x, rb.velocity.y, flatVel.z);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // プレイヤーの押し
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = (transform.position - collision.transform.position);
            pushDir.y = 0f;
            pushDir.Normalize();
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);
        }

        // FollowerBlockによる押し（isKinematic対応）
        else if (collision.gameObject.CompareTag(followerTag))
        {
            Rigidbody followerRb = collision.rigidbody;
            if (followerRb != null && followerRb.isKinematic)
            {
                // FollowerBlockの移動方向を検出
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
