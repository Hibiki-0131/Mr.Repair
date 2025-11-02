using UnityEngine;

public class PlayerRideHandler : MonoBehaviour
{
    private Transform currentPlatform;
    private Vector3 lastPlatformPos;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RepeatMoverBlock>() ||
            collision.gameObject.GetComponent<SlideBlock>())
        {
            currentPlatform = collision.transform;
            lastPlatformPos = currentPlatform.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (currentPlatform == null) return;

        // 現在のブロック移動量を求める
        Vector3 platformDelta = currentPlatform.position - lastPlatformPos;

        // プレイヤーをその分だけ動かす
        transform.position += platformDelta;

        // 次のフレーム用に記録更新
        lastPlatformPos = currentPlatform.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == currentPlatform)
        {
            currentPlatform = null;
        }
    }
}
