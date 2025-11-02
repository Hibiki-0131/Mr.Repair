using UnityEngine;
using System.Collections;

public class SlideBlock : MonoBehaviour
{
    [SerializeField] private string blockID;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private float obstacleCheckDistance = 0.6f; // これで前方チェック距離を調整

    private bool isMoving = false;
    private bool isAtStart = true;
    private Vector3 startPos;
    private Vector3 endPos;

    public string BlockID => blockID;

    private void Start()
    {
        startPos = transform.position;
        endPos = startPos + moveDirection.normalized * moveDistance;
    }

    public void Activate()
    {
        if (!isMoving)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    private IEnumerator SlideRoutine()
    {
        // 現在どちら側へ動くか判定
        Vector3 dir = isAtStart ? moveDirection.normalized : -moveDirection.normalized;

        // ?? 前方に障害物があるかチェック
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, obstacleCheckDistance + 0.1f))
        {
            // 自分自身は無視
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                Debug.Log($"{name}: 前方に {hit.collider.name} があり、動けません。");
                yield break; // 動作中止
            }
        }

        isMoving = true;
        Vector3 from = isAtStart ? startPos : endPos;
        Vector3 to = isAtStart ? endPos : startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;
        isAtStart = !isAtStart;
        isMoving = false;
    }

#if UNITY_EDITOR
    // SceneビューでRayの可視化（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection.normalized * obstacleCheckDistance);
    }
#endif
}
