using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;
    private bool isSettled = false;

    private RoomBuilder ownerBuilder;

    // RoomBuilder Ç©ÇÁíçì¸
    public void SetOwner(RoomBuilder builder)
    {
        ownerBuilder = builder;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.mass = 20f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (isSettled) return;

        rb.AddForce(
            Physics.gravity * gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSettled)
            return;

        if (!collision.gameObject.CompareTag("Ground"))
            return;

        var ownerTag =
            collision.collider.GetComponent<RoomColliderOwner>();

        if (ownerTag == null || ownerTag.Owner == null)
            return;

        ownerBuilder = ownerTag.Owner;
        TrySettleIntoHole();
    }

    private void TrySettleIntoHole()
    {
        if (ownerBuilder == null || ownerBuilder.SolidGrid == null)
            return;

        Vector3 localPos = transform.localPosition;

        int x = Mathf.FloorToInt(localPos.x / ownerBuilder.VoxelSize);
        int y = Mathf.FloorToInt(localPos.y / ownerBuilder.VoxelSize) - ownerBuilder.YOffset;
        int z = Mathf.FloorToInt(localPos.z / ownerBuilder.VoxelSize);

        if (x < 0 || y < 0 || z < 0 ||
            x >= ownerBuilder.SolidGrid.GetLength(0) ||
            y >= ownerBuilder.SolidGrid.GetLength(1) ||
            z >= ownerBuilder.SolidGrid.GetLength(2))
            return;

        if (ownerBuilder.SolidGrid[x, y, z])
            return;

        ownerBuilder.FillHole(x, y, z);

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Åö äÆëSÇ…ÉZÉãíÜêSÇ÷å≈íË
        transform.localPosition = new Vector3(
            (x + 0.5f) * ownerBuilder.VoxelSize,
            (y + ownerBuilder.YOffset + 0.5f) * ownerBuilder.VoxelSize,
            (z + 0.5f) * ownerBuilder.VoxelSize
        );

        isSettled = true;
    }
}
