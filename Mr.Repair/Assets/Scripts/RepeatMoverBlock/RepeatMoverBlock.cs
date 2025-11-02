using UnityEngine;
using System.Collections;

public class RepeatMoverBlock : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private string blockID;
    [SerializeField] private float moveSpeed = 2f;   // 移動速度
    [SerializeField] private float waitTime = 0.5f;  // 停止時間

    [Header("移動パターン設定")]
    [Tooltip("移動方向リスト（順番に移動）")]
    [SerializeField]
    private Vector3[] moveDirections = new Vector3[]
    {
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
    };

    [Tooltip("各方向に対応する移動距離（moveDirectionsと同じ数に）")]
    [SerializeField]
    private float[] moveDistances = new float[]
    {
        1f,
        2f
    };

    private bool isRepeating = false;
    private bool isMoving = false;
    private int currentIndex = 0;
    private Vector3 currentStartPos;  // ← 現在のスタート位置を記録
    private Coroutine routine;

    public string BlockID => blockID;

    private void Start()
    {
        // 起動時の位置を初期スタート位置として記録
        currentStartPos = transform.position;
    }

    /// <summary>
    /// 動作開始・停止を切り替える
    /// </summary>
    public void ToggleRepeat()
    {
        if (isRepeating)
        {
            // 停止処理
            StopRepeating();
        }
        else
        {
            // 再開処理
            StartRepeating();
        }
    }

    private void StartRepeating()
    {
        if (routine != null)
            StopCoroutine(routine);

        isRepeating = true;
        isMoving = true;
        routine = StartCoroutine(RepeatRoutine());
    }

    private void StopRepeating()
    {
        if (routine != null)
            StopCoroutine(routine);

        isRepeating = false;
        isMoving = false;

        // 現在位置を新しいスタート地点に
        currentStartPos = transform.position;
    }

    private IEnumerator RepeatRoutine()
    {
        while (isRepeating)
        {
            if (moveDirections.Length == 0 || moveDirections.Length != moveDistances.Length)
            {
                Debug.LogWarning($"[{name}] moveDirections と moveDistances の要素数が一致していません。");
                yield break;
            }

            // 現在の方向・距離を取得
            Vector3 dir = moveDirections[currentIndex].normalized;
            float distance = moveDistances[currentIndex];
            Vector3 target = currentStartPos + dir * distance;

            // 移動
            yield return MoveBetween(currentStartPos, target);
            yield return new WaitForSeconds(waitTime);

            // スタート位置を更新（今の位置を次の基準にする）
            currentStartPos = transform.position;

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
