using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalTrigger : MonoBehaviour
{
    private bool triggered = false;

    [SerializeField] private SoundTrigger soundTrigger;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Start()
    {
        if (soundTrigger == null)
            soundTrigger = FindObjectOfType<SoundTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        var presentation = other.GetComponent<PlayerPresentationController>();
        var clear = other.GetComponent<PlayerClearHandler>();

        if (!presentation || !clear) return;

        //-----------------------------------
        // Trigger中心座標
        //-----------------------------------
        var col = GetComponent<BoxCollider>();

        Vector3 center = transform.TransformPoint(col.center);
        center.y = other.transform.position.y;

        //-----------------------------------
        // ★クリア演出終了後にSE再生
        //-----------------------------------
        soundTrigger?.PlayByKey("GoalSE", center);  // ←ここが追加ポイント
        presentation.PlayGoal(center, () =>
        {
            clear.Clear();
        });

        triggered = true;
    }
}
