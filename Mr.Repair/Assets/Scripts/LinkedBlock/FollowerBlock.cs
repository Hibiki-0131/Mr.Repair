using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FollowerBlock : MonoBehaviour
{
    [SerializeField] private string groupID;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask powerBlockMask; // ← PowerBlock用レイヤーを指定

    private Rigidbody rb;
    private Collider col;
    private Vector3 targetPosition;

    public string GroupID => groupID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.isKinematic = true;
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
        float distance = delta.magnitude;

        // 前方に壁があればその手前で止まる
        if (rb.SweepTest(direction, out RaycastHit hit, distance))
        {
            targetPosition = rb.position + direction * (hit.distance - 0.01f);
        }
        else
        {
            targetPosition += delta;
        }
    }

    private void FixedUpdate()
    {
        // 壁やPowerBlockとめり込んでいないかチェック
        ResolveOverlap();

        // スムーズ移動
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }

    /// <summary>
    /// PowerBlockや他のFollowerとのめり込みを検出して押し戻す
    /// </summary>
    private void ResolveOverlap()
    {
        Collider[] overlaps = Physics.OverlapBox(
            col.bounds.center,
            col.bounds.extents * 0.98f,
            transform.rotation,
            powerBlockMask
        );

        foreach (var overlap in overlaps)
        {
            if (overlap == col) continue;

            Vector3 dir = (transform.position - overlap.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, overlap.transform.position);
            float minDist = col.bounds.extents.x + overlap.bounds.extents.x;

            if (distance < minDist)
            {
                // 軽く押し戻す
                Vector3 push = dir * (minDist - distance + 0.001f);
                rb.MovePosition(rb.position + push);
                targetPosition = rb.position; // 補正位置を追従目標に反映
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(col ? col.bounds.center : transform.position, (col ? col.bounds.size : Vector3.one) * 0.98f);
    }
#endif
}
