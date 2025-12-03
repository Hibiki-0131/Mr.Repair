using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;
    private bool settled = false;
    private RoomBuilder builder;

    public void Init(RoomBuilder room)
    {
        builder = room;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 20f;
    }

    private void FixedUpdate()
    {
        if (!settled)
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

        // 穴に入った条件を満たすと埋める
        if (!settled && ShouldSettle())
        {
            SettleIntoHole();
        }
    }

    /// <summary>
    /// 穴にハマったかどうかを判定する
    /// </summary>
    private bool ShouldSettle()
    {
        if (builder == null) return false;

        // voxel 座標に変換
        Vector3 local = builder.transform.InverseTransformPoint(transform.position);

        int vx = Mathf.RoundToInt(local.x / builder.voxelSize);
        int vy = Mathf.RoundToInt(local.y / builder.voxelSize);
        int vz = Mathf.RoundToInt(local.z / builder.voxelSize);

        // 範囲外は無視
        if (vx < 0 || vx >= builder.width) return false;
        if (vy < 0 || vy >= builder.heightCount) return false;
        if (vz < 0 || vz >= builder.depth) return false;

        // ★ 穴の判定（solid=false → 穴）
        if (builder.solid[vx, vy, vz] == true)
            return false;

        // 地面に触ったときのみ埋まる（暴走防止）
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

    private void SettleIntoHole()
    {
        settled = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        Vector3 local = builder.transform.InverseTransformPoint(transform.position);

        int vx = Mathf.RoundToInt(local.x / builder.voxelSize);
        int vy = Mathf.RoundToInt(local.y / builder.voxelSize);
        int vz = Mathf.RoundToInt(local.z / builder.voxelSize);

        // 穴を埋める
        builder.SetSolidVoxel(vx, vy, vz, true);

        // コライダー再構築（床と一体化）
        builder.RebuildColliders();

        Debug.Log($"CarryBlock settled at voxel ({vx},{vy},{vz})");
    }
}
