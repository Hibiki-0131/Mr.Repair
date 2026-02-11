using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class RepeatMoverBlock : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private string blockID;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 0.5f;

    [Header("移動パターン設定")]
    [SerializeField]
    private Vector3[] moveDirections = new Vector3[]
    {
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
    };

    [SerializeField]
    private float[] moveDistances = new float[]
    {
        1f,
        2f
    };

    private Rigidbody rb;

    private bool isRepeating = false;
    private bool isMoving = false;
    private int currentIndex = 0;

    private Vector3 currentStartPos;
    private Coroutine routine;

    public string BlockID => blockID;

    // ===============================
    // 初期化
    // ===============================
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        currentStartPos = rb.position;
    }

    // ===============================
    // 外部トグル
    // ===============================
    public void ToggleRepeat()
    {
        if (isRepeating)
            StopRepeating();
        else
            StartRepeating();
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

        currentStartPos = rb.position;
    }

    // ===============================
    // ループ処理
    // ===============================
    private IEnumerator RepeatRoutine()
    {
        while (isRepeating)
        {
            if (moveDirections.Length == 0 ||
                moveDirections.Length != moveDistances.Length)
            {
                Debug.LogWarning($"[{name}] 移動設定の配列数が一致していません");
                yield break;
            }

            Vector3 dir = moveDirections[currentIndex].normalized;
            float distance = moveDistances[currentIndex];

            Vector3 target = currentStartPos + dir * distance;

            yield return MoveBetween(currentStartPos, target);
            yield return new WaitForSeconds(waitTime);

            currentStartPos = rb.position;
            currentIndex = (currentIndex + 1) % moveDirections.Length;
        }

        isMoving = false;
    }

    // ===============================
    // 物理同期移動
    // ===============================
    private IEnumerator MoveBetween(Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime * moveSpeed;

            Vector3 nextPos = Vector3.Lerp(from, to, t);
            rb.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(to);
    }

#if UNITY_EDITOR
    // ===============================
    // Gizmo表示
    // ===============================
    private void OnDrawGizmosSelected()
    {
        if (moveDirections == null || moveDistances == null)
            return;

        Gizmos.color = Color.cyan;

        Vector3 previewPos = transform.position;

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
