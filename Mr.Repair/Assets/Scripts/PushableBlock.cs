using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [Header("Prefab Reference (é©ìÆê›íË)")]
    public GameObject prefabReference;

    [Header("Gravity Settings")]
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

        // Prefab éQè∆Ç™ãÛÇ»ÇÁé©ï™ÇÃ prefab root Çï€ë∂
        if (prefabReference == null)
            prefabReference = gameObject;
    }

    private void Start()
    {
        ResettableStageController.Instance?.RegisterCarryBlock(this);
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

    private void TrySettleIntoHole()
    {
        var builder = RoomBuilder.Instance;
        if (builder == null || builder.SolidGrid == null)
            return;

        Vector3 pos = transform.position;
        int x = Mathf.RoundToInt(pos.x / builder.VoxelSize);
        int z = Mathf.RoundToInt(pos.z / builder.VoxelSize);
        int y = 0;

        if (x < 0 || x >= builder.SolidGrid.GetLength(0)) return;
        if (z < 0 || z >= builder.SolidGrid.GetLength(2)) return;

        if (!builder.SolidGrid[x, y, z])
        {
            builder.FillHole(x, y, z);

            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            isSettled = true;
        }
    }
}
