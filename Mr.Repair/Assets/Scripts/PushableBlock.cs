using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [Header("Prefab Reference（固定）")]
    public GameObject prefabReference;

    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;
    private bool isSettled = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 20f;

#if UNITY_EDITOR
        // Editor時のみ PrefabReference を初期設定
        if (prefabReference == null)
        {
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefab != null)
                prefabReference = prefab;
        }
#endif
    }

    private void FixedUpdate()
    {
        if (!isSettled)
        {
            rb.AddForce(
                Physics.gravity * gravityMultiplier,
                ForceMode.Acceleration
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSettled)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            TrySettleIntoHole();
        }
    }

    /// <summary>
    /// ★ local 座標前提で穴埋め判定を行う
    /// </summary>
    private void TrySettleIntoHole()
    {
        var builder = RoomBuilder.Instance;
        if (builder == null || builder.SolidGrid == null)
            return;

        // ★ world ではなく local を使用
        Vector3 localPos = transform.localPosition;

        int x = Mathf.RoundToInt(localPos.x / builder.VoxelSize);
        int z = Mathf.RoundToInt(localPos.z / builder.VoxelSize);
        int y = 0; // 現在は床レイヤー固定

        // 安全な範囲チェック
        if (x < 0 || z < 0 ||
            x >= builder.SolidGrid.GetLength(0) ||
            z >= builder.SolidGrid.GetLength(2))
            return;

        // 穴なら埋める
        if (!builder.SolidGrid[x, y, z])
        {
            builder.FillHole(x, y, z);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // ★ 穴の中心へ正確にスナップ
            transform.localPosition =
                new Vector3(
                    x,
                    y + builder.YOffset,
                    z
                ) * builder.VoxelSize;

            isSettled = true;
        }
    }
}
