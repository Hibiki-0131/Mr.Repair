using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StartTrigger : MonoBehaviour
{
    [SerializeField] private float lockDuration = 2f;

    private bool triggered;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Start()
    {
        // シーン開始時に即ロックしたいなら
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) return;

        var presentation = player.GetComponent<PlayerPresentationController>();
        if (presentation == null) return;

        presentation.LockControl(lockDuration);

        triggered = true;
    }

    // トリガー式にしたい場合はこちら
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        var presentation = other.GetComponent<PlayerPresentationController>();
        if (presentation == null) return;

        presentation.LockControl(lockDuration);
        triggered = true;
    }
    */
}
