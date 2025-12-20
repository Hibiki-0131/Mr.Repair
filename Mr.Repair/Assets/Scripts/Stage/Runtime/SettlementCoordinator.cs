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
        colliderScheduler.RequestRebuild();
    }

    private void TrySettle(PushableBlock block)
    {
        if (!terrain.TryFillFromCarryBlock(
            block.transform.localPosition,
            out Vector3 snapped))
            return;

        block.transform.localPosition = snapped;
        block.Freeze();

        // š ‚±‚ÌƒtƒŒ[ƒ€‚Å‚ÍÄ\’z‚µ‚È‚¢
        colliderScheduler.RequestRebuild();
    }


    // ================================
    // Reset / Rebind API
    // ================================

    public void SetTerrain(TerrainState terrain)
    {
        this.terrain = terrain;
    }

}
