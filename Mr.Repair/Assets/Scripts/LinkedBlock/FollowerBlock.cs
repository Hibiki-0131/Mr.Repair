using UnityEngine;

public class FollowerBlock : MonoBehaviour
{
    [SerializeField] private string groupID;
    [SerializeField] private float moveSpeed = 10f;
    private Rigidbody rb;
    private Vector3 targetPosition;
    public string GroupID => groupID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Start()
    {
        BlockLinkManager.Instance.RegisterFollower(this);
        targetPosition = transform.position;
    }

    // 動ける割合(0.0~1.0)を返す
    public float CheckMovementRatio(Vector3 delta)
    {
        float distance = delta.magnitude;
        if (distance < 0.0001f) return 1f;

        Vector3 direction = delta.normalized;
        // 床に埋まらないよう、判定の箱を少し上にオフセットし、サイズを0.9倍にする
        Vector3 center = transform.position + Vector3.up * 0.2f;
        Vector3 halfExtents = (transform.localScale * 0.45f);

        RaycastHit[] hits = Physics.BoxCastAll(
            center,
            halfExtents,
            direction,
            transform.rotation,
            distance + 0.05f, // 少し先まで見る
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );

        float minRatio = 1f;

        foreach (var hit in hits)
        {
            // 自分自身、または親（ContentRoot）なら無視
            if (hit.collider.gameObject == gameObject || hit.transform.IsChildOf(transform.parent)) continue;

            // ヒット距離から動ける割合を算出（0.01fは壁との隙間）
            float hitDistance = Mathf.Max(0, hit.distance - 0.01f);
            float ratio = hitDistance / distance;

            if (ratio < minRatio)
            {
                minRatio = ratio;
                Debug.Log($"[Follower] {hit.collider.name} に衝突。移動を制限します。");
            }
        }
        return minRatio;
    }

    public void ApplyMovement(Vector3 finalDelta)
    {
        targetPosition = rb.position + finalDelta;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }
}