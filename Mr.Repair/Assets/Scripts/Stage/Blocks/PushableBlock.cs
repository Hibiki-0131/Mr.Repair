using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.mass = 20f;
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
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

    // ================================
    // Settlement 用 API
    // ================================
    public void Freeze()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void Unfreeze()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
    }

    // ================================
    // 互換 API（旧 RoomBuilder 用）
    // ================================
    public void SetOwner(RoomBuilder builder)
    {
        // 現在は参照しない（互換のためだけに存在）
    }
}
