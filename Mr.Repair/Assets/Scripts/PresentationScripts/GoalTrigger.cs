using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        Debug.Log("<color=red>[GoalTrigger] Ready</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        var presentation = other.GetComponent<PlayerPresentationController>();
        var clear = other.GetComponent<PlayerClearHandler>();

        if (!presentation || !clear) return;

        //-----------------------------------
        // ★ Trigger中心座標取得
        //-----------------------------------
        var col = GetComponent<BoxCollider>();

        Vector3 center = transform.TransformPoint(col.center);

        // yは現在値を維持（床にめり込まないため）
        center.y = other.transform.position.y;

        presentation.PlayGoal(center, () =>
        {
            clear.Clear();
        });

        triggered = true;
    }

}
