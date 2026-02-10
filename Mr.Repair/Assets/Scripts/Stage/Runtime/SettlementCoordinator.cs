using System.Collections.Generic;
using UnityEngine;

public class SettlementCoordinator : MonoBehaviour
{
    private StageContext context;
    private readonly List<PushableBlock> pending = new();
    private bool pendingRebuild;

    public void SetContext(StageContext context) => this.context = context;

    public void Enqueue(PushableBlock block)
    {
        if (!pending.Contains(block)) pending.Add(block);
    }

    private void LateUpdate()
    {
        if (pending.Count == 0) return;

        foreach (var block in pending)
            TrySettle(block);

        pending.Clear();

        if (pendingRebuild)
        {
            Debug.Log("[SettlementCoordinator] Rebuild requested"); // Åöí«â¡

            if (context == null)
                Debug.LogError("[SettlementCoordinator] context is NULL");

            context.RequestColliderRebuild();
            pendingRebuild = false;
        }
    }


    private void TrySettle(PushableBlock block)
    {
        if (block.IsSettled || context?.Terrain == null) return;

        Rigidbody rb = block.GetComponent<Rigidbody>();
        if (rb.velocity.sqrMagnitude > 0.01f) return;

        Vector3 localPos = block.transform.localPosition;

        if (!context.Terrain.TryFillFromCarryBlock(localPos, out Vector3 snapped))
            return;

        block.transform.localPosition = snapped;
        block.Freeze();
        block.MarkSettled();

        // Åö GridÇæÇØçXêVÅBPrefabê∂ê¨ã÷é~
        pendingRebuild = true;
    }

}