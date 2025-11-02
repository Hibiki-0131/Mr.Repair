using UnityEngine;
using System.Collections;

public class RepeatMoverBlock : MonoBehaviour
{
    [SerializeField] private string blockID;           // スイッチと対応するID
    [SerializeField] private float moveDistance = 2f;  // 移動距離
    [SerializeField] private float moveSpeed = 2f;     // 移動速度
    [SerializeField] private Vector3 moveDirection = Vector3.forward; // 移動方向
    [SerializeField] private float waitTime = 0.5f;    // 行きと戻りの間の待機時間

    private bool isMoving = false;     // コルーチン動作中フラグ
    private bool isRepeating = false;  // 繰り返し中フラグ
    private Vector3 startPos;
    private Vector3 endPos;

    public string BlockID => blockID;

    private void Start()
    {
        startPos = transform.position;
        endPos = startPos + moveDirection.normalized * moveDistance;
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
            // 前進
            yield return MoveBetween(startPos, endPos);
            yield return new WaitForSeconds(waitTime);

            // 戻る
            yield return MoveBetween(endPos, startPos);
            yield return new WaitForSeconds(waitTime);
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
}
