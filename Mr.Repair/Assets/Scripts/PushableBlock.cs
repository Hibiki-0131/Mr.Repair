using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableBlock : MonoBehaviour
{
    [Header("押す設定")]
    [SerializeField] private float pushForce = 8f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float stepHeight = 0.3f; // ← 乗り越え可能な段差の高さ
    [SerializeField] private float stepCheckDistance = 0.6f; // プレイヤーとの距離
    [SerializeField] private PhysicMaterial smoothMaterial;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 5f;
        rb.angularDrag = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (smoothMaterial != null)
        {
            var col = GetComponent<Collider>();
            col.material = smoothMaterial;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // 押す方向を決定
        Vector3 pushDir = collision.transform.forward;
        pushDir.y = 0f;
        pushDir.Normalize();

        // 段差乗り越えチェック
        if (CanStep(pushDir))
        {
            // 乗り越え時：少し上方向の補助力を加える
            rb.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
        }

        // 通常の押し処理
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// 前方に小さな段差があるかをレイキャストで判定
    /// </summary>
    private bool CanStep(Vector3 pushDir)
    {
        Vector3 originLower = transform.position + Vector3.up * 0.05f; // 下の位置
        Vector3 originUpper = transform.position + Vector3.up * stepHeight; // 上の位置

        // 下のレイがヒットして、上のレイがヒットしないとき → 段差あり
        if (Physics.Raycast(originLower, pushDir, out RaycastHit lowerHit, stepCheckDistance))
        {
            if (!Physics.Raycast(originUpper, pushDir, stepCheckDistance))
            {
                // 小さな段差とみなす
                return true;
            }
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Vector3 originLower = transform.position + Vector3.up * 0.05f;
        Vector3 originUpper = transform.position + Vector3.up * stepHeight;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(originLower, originLower + transform.forward * stepCheckDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(originUpper, originUpper + transform.forward * stepCheckDistance);
    }
#endif
}
