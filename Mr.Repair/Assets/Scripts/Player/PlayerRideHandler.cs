using UnityEngine;

public class PlayerRideHandler : MonoBehaviour
{
    [Header("足元チェック設定")]
    [SerializeField] private float recheckDistance = 0.3f;
    [SerializeField] private float platformStayGrace = 0.25f; // 一瞬離れても落ちない猶予時間

    private Transform currentPlatform;
    private Vector3 lastPlatformPos;
    private float lostTimer = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        TrySetPlatform(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        TrySetPlatform(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        // すぐには切らず、一定時間様子を見る
        if (collision.transform == currentPlatform)
        {
            lostTimer = platformStayGrace;
        }
    }

    private void FixedUpdate()
    {
        // プラットフォーム追従（FixedUpdate に統一）
        if (currentPlatform != null)
        {
            Vector3 delta = currentPlatform.position - lastPlatformPos;
            transform.position += delta;
            lastPlatformPos = currentPlatform.position;
        }

        // 接触が切れてもすぐには解除しない
        if (lostTimer > 0f)
        {
            lostTimer -= Time.fixedDeltaTime;
            if (lostTimer <= 0f)
                currentPlatform = null;
        }

        // ブロックの再検出
        if (currentPlatform == null)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, out RaycastHit hit, recheckDistance))
            {
                if (hit.transform.GetComponent<RepeatMoverBlock>() ||
                    hit.transform.GetComponent<SlideBlock>())
                {
                    currentPlatform = hit.transform;
                    lastPlatformPos = currentPlatform.position;
                }
            }
        }
    }

    private void TrySetPlatform(Collision collision)
    {
        if (collision.gameObject.GetComponent<RepeatMoverBlock>() ||
            collision.gameObject.GetComponent<SlideBlock>())
        {
            currentPlatform = collision.transform;
            lastPlatformPos = currentPlatform.position;
            lostTimer = 0f;
        }
    }
}
