using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
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

    // ========================
    // Åö Reset óp API
    // ========================
    public void ResetBlock(Vector3 pos, Quaternion rot)
    {
        isSettled = false;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(pos, rot);
    }

    public void MarkAsSettled()
    {
        isSettled = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // =========================
    // Åö åäÇ…óéÇøÇΩÇ∆Ç´
    // =========================
    private void OnCollisionEnter(Collision collision)
    {
        if (isSettled) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            TrySettleIntoHole();
        }
    }

    private void TrySettleIntoHole()
    {
        var builder = RoomBuilder.Instance;
        if (builder == null || builder.SolidGrid == null) return;

        Vector3 pos = transform.position;
        int x = Mathf.RoundToInt(pos.x / builder.VoxelSize);
        int z = Mathf.RoundToInt(pos.z / builder.VoxelSize);
        int y = 0;

        if (x < 0 || x >= builder.SolidGrid.GetLength(0)) return;
        if (y < 0 || y >= builder.SolidGrid.GetLength(1)) return;
        if (z < 0 || z >= builder.SolidGrid.GetLength(2)) return;

        if (!builder.SolidGrid[x, y, z])
        {
            builder.FillHole(x, y, z);
            MarkAsSettled();
        }
    }
}
