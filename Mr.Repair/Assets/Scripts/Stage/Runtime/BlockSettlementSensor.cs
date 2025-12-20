using System.Collections;
using UnityEngine;

public class BlockSettlementSensor : MonoBehaviour
{
    [SerializeField] private SettlementCoordinator coordinator;

    public void NotifyCollision(PushableBlock block)
    {
        StartCoroutine(WaitAndReport(block));
    }

    private IEnumerator WaitAndReport(PushableBlock block)
    {
        yield return new WaitForFixedUpdate();
        coordinator.Enqueue(block);
    }
}
