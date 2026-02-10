using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PushableBlock))]
public class BlockSettlementSensor : MonoBehaviour
{
    [SerializeField] private SettlementCoordinator coordinator;

    private PushableBlock block;

    private void Awake()
    {
        block = GetComponent<PushableBlock>();

        if (coordinator == null)
        {
            Debug.LogWarning(
                "[BlockSettlementSensor] Coordinator is not injected yet",
                this
            );
        }
    }

    /// <summary>
    /// StageInitializer から注入
    /// </summary>
    public void SetCoordinator(SettlementCoordinator coordinator)
    {
        this.coordinator = coordinator;

        Debug.Log(
            $"[BlockSettlementSensor] Coordinator injected: {coordinator.name}",
            this
        );
    }

    // =========================
    // ★ ここが最重要修正
    // Ground のみ反応
    // =========================
    private void OnCollisionEnter(Collision collision)
    {
        if (coordinator == null) return;

        // Groundタグ以外は完全無視
        if (!collision.collider.CompareTag("Ground"))
            return;

        Debug.Log(
            $"[Sensor] Ground collision → settlement request ({block.name})",
            this
        );

        StartCoroutine(WaitAndReport());
    }

    private IEnumerator WaitAndReport()
    {
        yield return new WaitForFixedUpdate();
        coordinator.Enqueue(block);
    }
}
