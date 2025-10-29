using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PowerBlock : MonoBehaviour
{
    [SerializeField] private string groupID;
    private Rigidbody rb;
    private Vector3 lastPosition;

    public string GroupID => groupID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        BlockLinkManager.Instance.RegisterPowerBlock(this);
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // –ˆƒtƒŒ[ƒ€AˆÚ“®—Ê‚ðŒvŽZ
        Vector3 delta = transform.position - lastPosition;

        if (delta.sqrMagnitude > 0.0001f)
        {
            // “¯‚¶ID‚ÌFollower‚½‚¿‚ÉˆÚ“®‚ð“`‚¦‚é
            BlockLinkManager.Instance.SyncFollowerMovement(groupID, delta);
        }

        lastPosition = transform.position;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    }
}
