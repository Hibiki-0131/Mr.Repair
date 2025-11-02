using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SlideBlock : MonoBehaviour
{
    [SerializeField] private string blockID;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private float obstacleCheckDistance = 0.6f;

    private bool isMoving = false;
    private bool isAtStart = true;
    private Vector3 startPos;
    private Vector3 endPos;
    private Rigidbody rb;

    public string BlockID => blockID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ← 他オブジェクトを弾かない
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        startPos = transform.position;
        endPos = startPos + moveDirection.normalized * moveDistance;
    }

    public void Activate()
    {
        if (!isMoving)
            StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        Vector3 dir = isAtStart ? moveDirection.normalized : -moveDirection.normalized;

        // 前方障害物チェック
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, obstacleCheckDistance + 0.1f))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                Debug.Log($"{name}: 前方に {hit.collider.name} があり、動作を中止。");
                yield break;
            }
        }

        isMoving = true;
        Vector3 from = isAtStart ? startPos : endPos;
        Vector3 to = isAtStart ? endPos : startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            Vector3 nextPos = Vector3.Lerp(from, to, t);
            rb.MovePosition(nextPos); // ← Transformではなく物理移動
            yield return null;
        }

        rb.MovePosition(to);
        isAtStart = !isAtStart;
        isMoving = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection.normalized * obstacleCheckDistance);
    }
#endif
}
