using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClearTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
