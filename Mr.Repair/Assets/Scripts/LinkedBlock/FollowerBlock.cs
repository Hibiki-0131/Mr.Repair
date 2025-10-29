using UnityEngine;

public class FollowerBlock : MonoBehaviour
{
    [SerializeField] private string groupID;
    private Rigidbody rb;
    private Vector3 targetPosition;

    public string GroupID => groupID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // •¨—‰‰Z‚ğØ‚éiˆÀ’è‰»j
        rb.interpolation = RigidbodyInterpolation.None; // •âŠÔ‚à–³Œø
    }

    private void Start()
    {
        BlockLinkManager.Instance.RegisterFollower(this);
        targetPosition = transform.position;
    }

    public void ApplyMovement(Vector3 delta)
    {
        targetPosition += delta;
    }

    private void FixedUpdate()
    {
        // MovePosition‚Å³Šm‚È•¨—“¯Šú
        rb.MovePosition(targetPosition);
    }
}
