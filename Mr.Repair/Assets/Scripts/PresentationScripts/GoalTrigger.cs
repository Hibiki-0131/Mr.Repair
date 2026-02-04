using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalTrigger : MonoBehaviour
{
    private bool triggered = false;
    private BoxCollider box;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        Debug.Log("<color=red>[GoalTrigger] Ready</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=red>[GoalTrigger] Enter : {other.name}</color>");

        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        var presentation = other.GetComponent<PlayerPresentationController>();
        var playerClear = other.GetComponent<PlayerClearHandler>();

        if (!presentation || !playerClear)
        {
            Debug.LogError("[GoalTrigger] Required component missing");
            return;
        }

        //----------------------------------
        // ★中心座標取得（XZのみ使用）
        //----------------------------------
        Vector3 goalPos = box.bounds.center;

        Debug.Log($"<color=red>[GoalTrigger] Move Target : {goalPos}</color>");

        presentation.PlayGoal(goalPos, () =>
        {
            Debug.Log("<color=red>[GoalTrigger] Clear()</color>");
            playerClear.Clear();
        });

        triggered = true;
    }
}
