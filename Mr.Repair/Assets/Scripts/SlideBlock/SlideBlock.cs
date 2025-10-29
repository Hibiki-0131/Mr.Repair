using UnityEngine;
using System.Collections;

public class SlideBlock : MonoBehaviour
{
    [SerializeField] private string blockID; // スイッチと対応するID
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;

    private bool isMoving = false;

    public string BlockID => blockID;

    public void Activate()
    {
        if (!isMoving)
            StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveDirection.normalized * moveDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }
}
