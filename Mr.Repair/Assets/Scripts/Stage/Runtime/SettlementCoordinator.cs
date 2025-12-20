using System.Collections.Generic;
using UnityEngine;

public class SettlementCoordinator : MonoBehaviour
{
    [SerializeField] private TerrainState terrain;
    [SerializeField] private ColliderRebuildScheduler colliderScheduler;

    private readonly List<PushableBlock> pending = new();

    public void Enqueue(PushableBlock block)
    {
        pending.Add(block);
    }

    private void LateUpdate()
    {
        if (pending.Count == 0) return;

        foreach (var block in pending)
            TrySettle(block);

        pending.Clear();
    }

    private void TrySettle(PushableBlock block)
    {
        var rb = block.GetComponent<Rigidbody>();
        if (rb.velocity.sqrMagnitude > 0.01f)
            return;

        Vector3 localPos = block.transform.localPosition;

        if (!terrain.TryFillFromCarryBlock(localPos, out Vector3 snapped))
            return;

        // ★ 中心判定（0.25マス以内）
        if ((localPos - snapped).sqrMagnitude > 0.0625f)
            return;

        block.transform.localPosition = snapped;
        block.Freeze();

        colliderScheduler.RequestRebuild();
    }

    public void SetTerrain(TerrainState terrain)
    {
        this.terrain = terrain;
    }
}
