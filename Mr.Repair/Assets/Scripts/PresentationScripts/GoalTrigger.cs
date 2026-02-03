using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Goal Animation Duration")]
    [SerializeField] private float duration = 3f;

    private bool triggered = false;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        var presentation = other.GetComponent<PlayerPresentationController>();
        if (presentation == null) return;

        presentation.PlayGoal(duration);

        triggered = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider>();
        if (!col) return;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);
    }
#endif
}
