using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
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
    }

    private void FixedUpdate()
    {
        if (!isSettled)
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 一度固定されたらもう処理しない
        if (isSettled) return;

        // GroundTag として RoomBuilder の床か他のブロックと接触したとき
        if (collision.gameObject.CompareTag("Ground"))
        {
            TrySettleIntoHole();
        }
    }

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

        // ★ 範囲チェック（これがないと例外が出る）
        if (x < 0 || x >= builder.SolidGrid.GetLength(0)) return;
        if (y < 0 || y >= builder.SolidGrid.GetLength(1)) return;
        if (z < 0 || z >= builder.SolidGrid.GetLength(2)) return;

        if (!builder.SolidGrid[x, y, z])
        {
            builder.FillHole(x, y, z);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            isSettled = true;

            Debug.Log("Block settled into hole.");
        }
    }

}
