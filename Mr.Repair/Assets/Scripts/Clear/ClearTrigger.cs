using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClearTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.name);  // ← デバッグ追加

        if (other.CompareTag("ClearCube"))
        {
            Debug.Log("Player detected!");
            var playerClear = other.GetComponent<PlayerClearHandler>();
            if (playerClear != null)
            {
                playerClear.Clear();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
    }
#endif
}
