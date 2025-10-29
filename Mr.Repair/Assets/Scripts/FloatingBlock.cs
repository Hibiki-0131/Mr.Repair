using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FloatingBlock : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.3f; // 地面との距離を測る
    [SerializeField] private float targetFloatHeight = 0.1f;   // 浮く高さ
    [SerializeField] private float liftForce = 20f;            // 浮かせる力
    [SerializeField] private LayerMask groundMask;             // 地面レイヤー

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        HandleFloating();
    }

    private void HandleFloating()
    {
        // 下方向にレイを飛ばして地面との距離を測定
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            float distance = hit.distance;

            // 目標の浮き高さより近い場合 → 押し上げ
            if (distance < targetFloatHeight)
            {
                float forcePercent = 1f - (distance / targetFloatHeight);
                rb.AddForce(Vector3.up * liftForce * forcePercent, ForceMode.Acceleration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // デバッグ可視化（Sceneビューで距離確認）
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
