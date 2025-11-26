using UnityEngine;
using System.Collections;

public class TransparentWall : MonoBehaviour
{
    [SerializeField] private float deactivateDelay = 1f; // 非アクティブ化までの秒数（1秒）

    private bool isDeactivating = false;

    private void OnCollisionEnter(Collision collision)
    {
        // PushableBlockと衝突したら
        if (collision.gameObject.GetComponent<PushableBlock>() != null && !isDeactivating)
        {
            StartCoroutine(DeactivateAfterDelay());
        }
    }

    private IEnumerator DeactivateAfterDelay()
    {
        isDeactivating = true;
        yield return new WaitForSeconds(deactivateDelay);
        gameObject.SetActive(false);
    }
}
