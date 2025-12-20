using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(BlockSettlementSensor))]
public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;
    private BlockSettlementSensor sensor;

    /// <summary>
    /// すでに地形として確定したか
    /// SettlementCoordinator から参照される
    /// </summary>
    public bool IsSettled { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sensor = GetComponent<BlockSettlementSensor>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.mass = 20f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        IsSettled = false;

        Debug.Log($"[PushableBlock] Awake ({name})");
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic)
            return;

        rb.AddForce(
            Physics.gravity * gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsSettled)
            return;

        Debug.Log(
            $"[PushableBlock] OnCollisionEnter with {collision.collider.name}",
            this
        );

        if (sensor != null)
        {
            sensor.NotifyCollision(this);
        }
        else
        {
            Debug.LogError(
                "[PushableBlock] BlockSettlementSensor not found",
                this
            );
        }
    }

    // ================================
    // Settlement 用 API
    // ================================

    public void Freeze()
    {
        Debug.Log($"[PushableBlock] Freeze ({name})", this);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void Unfreeze()
    {
        Debug.Log($"[PushableBlock] Unfreeze ({name})", this);

        rb.isKinematic = false;
        IsSettled = false;
    }

    public void MarkSettled()
    {
        IsSettled = true;
        Debug.Log($"[PushableBlock] MarkSettled ({name})", this);
    }

    // ================================
    // 互換 API（旧 RoomBuilder 用）
    // ================================
    public void SetOwner(RoomBuilder builder)
    {
        // 現在は参照しない（互換のためだけに存在）
    }
}
