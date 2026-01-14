using UnityEngine;

public class PowerBlock : MonoBehaviour
{
    [SerializeField] private string groupID;
    private Rigidbody rb;
    private Vector3 lastPosition;
    public string GroupID => groupID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Start()
    {
        BlockLinkManager.Instance.RegisterPowerBlock(this);
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 delta = transform.position - lastPosition;

        if (delta.sqrMagnitude > 0.00001f)
        {
            BlockLinkManager.Instance.SyncFollowerMovement(groupID, delta);
            lastPosition = transform.position;
        }
    }

    // Manager‚©‚çŒÄ‚Î‚ê‚é•â³ˆ—
    public void ForceReposition(Vector3 finalDelta)
    {
        // Follower‚ª“®‚¯‚½•ª‚¾‚¯‚µ‚©Power‚à“®‚©‚³‚È‚¢
        Vector3 correctedPos = lastPosition + finalDelta;
        rb.position = correctedPos;
        lastPosition = correctedPos;
    }
}