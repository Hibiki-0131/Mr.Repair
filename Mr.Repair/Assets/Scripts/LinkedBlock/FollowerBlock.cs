using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FollowerBlock : MonoBehaviour
{
    [SerializeField] private string groupID;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float collisionCheckDistance = 0.55f; // 壁チェック距離（ブロックサイズより少し短め）
    [SerializeField] private LayerMask wallMask;

    private Rigidbody rb;
    private Vector3 targetPosition;

    public string GroupID => groupID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ← キネマティックに戻す
        rb.interpolation = RigidbodyInterpolation.None;
    }

    private void Start()
    {
        BlockLinkManager.Instance.RegisterFollower(this);
        targetPosition = transform.position;
    }

    public void ApplyMovement(Vector3 delta)
    {
        Vector3 direction = delta.normalized;

        // ブロックの中心から少し上げてRayを飛ばす
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        // 進行方向に壁があるかチェック
        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, collisionCheckDistance, wallMask))
        {
            // 壁がすぐ前にある → 移動禁止
            return;
        }

        // 壁がなければ移動許可
        targetPosition += delta;
    }

    private void FixedUpdate()
    {
        // スムーズに追従
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }

#if UNITY_EDITOR
    // Scene上でRay確認できるように
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * collisionCheckDistance);
    }
#endif
}
