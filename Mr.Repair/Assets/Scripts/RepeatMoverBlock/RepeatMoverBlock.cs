using UnityEngine;
using System.Collections;

public class RepeatMoverBlock : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private string blockID;
    [SerializeField] private float moveSpeed = 2f;   // 移動速度
    [SerializeField] private float waitTime = 0.5f;  // 各移動の間の停止時間

    [Header("移動パターン設定")]
    [Tooltip("移動方向リスト（順番に移動）")]
    [SerializeField]
    private Vector3[] moveDirections = new Vector3[]
    {
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, -1, 0)
    };

    [Tooltip("各方向に対応する移動距離（moveDirectionsと同じ数に）")]
    [SerializeField]
    private float[] moveDistances = new float[]
    {
        1f, 2f, 1f, 2f
    };

    private bool isRepeating = false;
    private bool isMoving = false;
    private int currentIndex = 0;
    private Vector3 startPos;

    public string BlockID => blockID;

    private void Start()
    {
        startPos = transform.position;
    }

    public void ToggleRepeat()
    {
        if (isRepeating)
        {
            StopAllCoroutines();
            isRepeating = false;
            isMoving = false;
        }
        else
        {
            isRepeating = true;
            StartCoroutine(RepeatRoutine());
        }
    }

    private IEnumerator RepeatRoutine()
    {
        isMoving = true;

        while (isRepeating)
        {
            // 安全チェック（要素数が揃っていないとき）
            if (moveDirections.Length == 0 || moveDirections.Length != moveDistances.Length)
            {
                Debug.LogWarning($"[{name}] moveDirections と moveDistances の要素数が一致していません。");
                yield break;
            }

            // 今の移動方向と距離を取得
            Vector3 dir = moveDirections[currentIndex].normalized;
            float distance = moveDistances[currentIndex];
            Vector3 target = transform.position + dir * distance;

            // 移動
            yield return MoveBetween(transform.position, target);
            yield return new WaitForSeconds(waitTime);

            // 次の方向へ
            currentIndex = (currentIndex + 1) % moveDirections.Length;
        }

        isMoving = false;
    }

    private IEnumerator MoveBetween(Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.position = to;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (moveDirections == null || moveDistances == null)
            return;

        Gizmos.color = Color.cyan;
        Vector3 previewPos = Application.isPlaying ? transform.position : transform.position;
        for (int i = 0; i < Mathf.Min(moveDirections.Length, moveDistances.Length); i++)
        {
            Vector3 dir = moveDirections[i].normalized * moveDistances[i];
            Gizmos.DrawLine(previewPos, previewPos + dir);
            Gizmos.DrawSphere(previewPos + dir, 0.05f);
            previewPos += dir;
        }
    }
#endif
}
