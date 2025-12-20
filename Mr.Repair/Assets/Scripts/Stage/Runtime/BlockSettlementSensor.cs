using System.Collections;
using UnityEngine;

public class BlockSettlementSensor : MonoBehaviour
{
    [SerializeField] private SettlementCoordinator coordinator;

    private void Awake()
    {
        if (coordinator == null)
        {
            Debug.LogWarning(
                "[BlockSettlementSensor] Coordinator is not injected yet",
                this
            );
        }
    }

    /// <summary>
    /// StageInitializer Ç©ÇÁíçì¸Ç≥ÇÍÇÈ
    /// </summary>
    public void SetCoordinator(SettlementCoordinator coordinator)
    {
        this.coordinator = coordinator;

        Debug.Log(
            $"[BlockSettlementSensor] Coordinator injected: {coordinator.name}",
            this
        );
    }

    /// <summary>
    /// PushableBlock Ç©ÇÁåƒÇŒÇÍÇÈ
    /// </summary>
    public void NotifyCollision(PushableBlock block)
    {
        if (coordinator == null)
        {
            Debug.LogError(
                "[BlockSettlementSensor] Coordinator is null. Injection missing.",
                this
            );
            return;
        }

        Debug.Log(
            $"[Sensor] NotifyCollision from {block.name}",
            this
        );

        StartCoroutine(WaitAndReport(block));
    }

    private IEnumerator WaitAndReport(PushableBlock block)
    {
        yield return new WaitForFixedUpdate();

        Debug.Log(
            $"[Sensor] Report to SettlementCoordinator {block.name}",
            this
        );

        coordinator.Enqueue(block);
    }
}
