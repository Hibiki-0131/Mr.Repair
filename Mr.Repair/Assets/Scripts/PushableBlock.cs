using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableBlock : MonoBehaviour
{
    [Header("âüÇ∑ê›íË")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepCheckDistance = 0.6f;

    [Header("óéâ∫îªíËê›íË")]
    [SerializeField] private float fallCheckDistance = 0.6f;
    [SerializeField] private float fallGravityBoost = 10f;
    [SerializeField] private float snapThreshold = 0.02f;

    private Rigidbody rb;
    private bool isMoving = false;
    private bool isFalling = false;
    private Vector3 moveDir;
    private Vector3 targetPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.drag = 4f;
        rb.angularDrag = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveBlock();
        }

        if (!HasGround())
        {
            StartFalling();
        }
    }

    private void MoveBlock()
    {
        Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, targetPos) < stopThreshold)
            isMoving = false;
    }

    private bool HasGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, fallCheckDistance);
    }

    private void StartFalling()
    {
        if (!isFalling)
        {
            isFalling = true;
            rb.useGravity = true;
            rb.AddForce(Vector3.down * fallGravityBoost, ForceMode.Acceleration);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || isMoving || isFalling)
            return;

        Vector3 pushDir = collision.transform.forward;
        pushDir.y = 0f;
        pushDir.Normalize();

        // ëOï˚Ç…ï«Ç™Ç»ÇØÇÍÇŒà⁄ìÆó\ñÒ
        if (!Physics.Raycast(transform.position + Vector3.up * 0.2f, pushDir, 1f))
        {
            targetPos = rb.position + pushDir;
            isMoving = true;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.05f,
                        transform.position + Vector3.up * 0.05f + Vector3.down * fallCheckDistance);
    }
#endif
}
