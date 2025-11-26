using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerDeath = other.GetComponent<PlayerDeathHandler>();
            if (playerDeath != null)
            {
                playerDeath.Die();
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
