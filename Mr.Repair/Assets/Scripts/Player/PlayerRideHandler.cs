using UnityEngine;

public class PlayerRideHandler : MonoBehaviour
{
    [Header("足元チェック設定")]
    [SerializeField] private float recheckDistance = 0.2f; // Collider変化時にも下のブロックを再検知できる距離

    private Transform currentPlatform;
    private Vector3 lastPlatformPos;
    private float platformLostTimer = 0f;
    private const float platformLostDelay = 0.1f; // 一瞬の接触切れを無視する時間

    private void OnCollisionEnter(Collision collision)
    {
        TrySetPlatform(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (currentPlatform == null) TrySetPlatform(collision);
        if (currentPlatform == null) return;

        Vector3 platformDelta = currentPlatform.position - lastPlatformPos;
        transform.position += platformDelta;
        lastPlatformPos = currentPlatform.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == currentPlatform)
        {
            // 一瞬の接触切れ（しゃがみなど）に対応するため、遅延解除
            platformLostTimer = platformLostDelay;
        }
    }

    private void FixedUpdate()
    {
        // 足元再チェック（しゃがみ時のCollider変更にも対応）
        if (currentPlatform == null || platformLostTimer > 0f)
        {
            platformLostTimer -= Time.fixedDeltaTime;
            if (platformLostTimer <= 0f)
            {
                RaycastHit hit;
                Vector3 origin = transform.position + Vector3.up * 0.05f;
                if (Physics.Raycast(origin, Vector3.down, out hit, recheckDistance))
                {
                    if (hit.transform.GetComponent<RepeatMoverBlock>() ||
                        hit.transform.GetComponent<SlideBlock>())
                    {
                        currentPlatform = hit.transform;
                        lastPlatformPos = currentPlatform.position;
                        return;
                    }
                }

                currentPlatform = null; // 足元にブロックがない
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
            platformLostTimer = 0f;
        }
    }
}
