using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableBlock : MonoBehaviour
{
    [Header("押す設定")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepCheckDistance = 0.6f;

    [Header("落下判定設定")]
    [SerializeField] private float fallCheckDistance = 0.6f;
    [SerializeField] private float fallGravityBoost = 10f;
    [SerializeField] private float snapThreshold = 0.02f;

    [Header("連動ブロックとの衝突設定")]
    [SerializeField] private string followerTag = "FollowerBlock";  // ← FollowerBlockのTag
    [SerializeField] private float followerPushForce = 6f;          // ← 押されたときの力の強さ
    [SerializeField] private float followerPushCooldown = 0.2f;     // ← 連続押し防止

    private Rigidbody rb;
    private bool isMoving = false;
    private bool isFalling = false;
    private bool recentlyPushed = false;
    private Vector3 moveDir;
    private Vector3 targetPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.drag = 4f;
        rb.angularDrag = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveBlock();
        }

        if (!HasGround())
        {
            StartFalling();
        }
    }

    private void MoveBlock()
    {
        Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, targetPos) < stopThreshold)
            isMoving = false;
    }

    private bool HasGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, fallCheckDistance);
    }

    private void StartFalling()
    {
        if (!isFalling)
        {
            isFalling = true;
            rb.useGravity = true;
            rb.AddForce(Vector3.down * fallGravityBoost, ForceMode.Acceleration);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isFalling)
            return;

        // FollowerBlock から押された場合
        if (collision.gameObject.CompareTag(followerTag))
        {
            if (!recentlyPushed)
            {
                Vector3 pushDir = (transform.position - collision.transform.position);
                pushDir.y = 0f;
                pushDir.Normalize();

                TryMoveByFollower(pushDir);
                StartCoroutine(ResetFollowerPush());
            }
            return;
        }

        // 通常のPlayerによる押し処理
        if (collision.gameObject.CompareTag("Player") && !isMoving)
        {
            Vector3 pushDir = collision.transform.forward;
            pushDir.y = 0f;
            pushDir.Normalize();

            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, pushDir, 1f))
                return;

            Vector3 frontCheckOrigin = transform.position + pushDir * stepCheckDistance + Vector3.up * 0.1f;

            if (Physics.Raycast(frontCheckOrigin, Vector3.down, out RaycastHit hitDown, 1f))
            {
                float heightDiff = hitDown.point.y - transform.position.y;

                if (heightDiff > stepHeight * 0.8f)
                    return;

                Vector3 adjustedTarget = new Vector3(
                    rb.position.x + pushDir.x,
                    transform.position.y + heightDiff,
                    rb.position.z + pushDir.z
                );

                targetPos = adjustedTarget;
                isMoving = true;
            }
        }
    }

    /// <summary>
    /// FollowerBlock から押されたときの移動処理
    /// </summary>
    private void TryMoveByFollower(Vector3 pushDir)
    {
        // 前方に障害物がある場合は動かない
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, pushDir, 0.9f))
            return;

        Vector3 target = rb.position + pushDir.normalized * 1f;

        rb.AddForce(pushDir * followerPushForce, ForceMode.VelocityChange);
        targetPos = target;
        isMoving = true;
    }

    private System.Collections.IEnumerator ResetFollowerPush()
    {
        recentlyPushed = true;
        yield return new WaitForSeconds(followerPushCooldown);
        recentlyPushed = false;
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
