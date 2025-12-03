using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [Header("Original Prefab (Reset 用)")]
    public GameObject prefabReference;   // ★ Reset に必要

    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;
    private bool isSettled = false; // 穴にハマった後は true

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 20f;

        // ★ prefabReference が空なら、自動で記録する（Editor のみ）
#if UNITY_EDITOR
        if (prefabReference == null)
        {
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefab != null)
            {
                prefabReference = prefab;
            }
        }
#endif
    }

    private void FixedUpdate()
    {
        if (!isSettled)
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSettled) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            TrySettleIntoHole();
        }
    }


    // ----------------------------------------------------
    // ★ ResettableStageController / RoomBuilder が利用する API
    // ----------------------------------------------------

    public void SetPrefabReference(GameObject prefab)
    {
        prefabReference = prefab;
    }

    public void MarkAsSettled()
    {
        isSettled = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }


    // ----------------------------------------------------
    // ★ 穴にはまって固定される処理
    // ----------------------------------------------------

    private void TrySettleIntoHole()
    {
        var builder = RoomBuilder.Instance;
        if (builder == null)
        {
            Debug.LogError("RoomBuilder.Instance が見つかりません");
            return;
        }

        if (builder.SolidGrid == null)
        {
            Debug.LogError("RoomBuilder.SolidGrid が初期化されていません");
            return;
        }

        Vector3 pos = transform.position;
        int x = Mathf.RoundToInt(pos.x / builder.VoxelSize);
        int z = Mathf.RoundToInt(pos.z / builder.VoxelSize);
        int y = 0;

        // ★ 範囲チェック
        if (x < 0 || x >= builder.SolidGrid.GetLength(0)) return;
        if (y < 0 || y >= builder.SolidGrid.GetLength(1)) return;
        if (z < 0 || z >= builder.SolidGrid.GetLength(2)) return;

        // ★ 落ちたマスが「穴」なら埋める
        if (!builder.SolidGrid[x, y, z])
        {
            builder.FillHole(x, y, z);

            MarkAsSettled();
            Debug.Log("Block settled into hole.");
        }
    }
}
