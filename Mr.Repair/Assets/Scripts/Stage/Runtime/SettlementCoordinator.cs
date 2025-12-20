using System.Collections.Generic;
using UnityEngine;

public class SettlementCoordinator : MonoBehaviour
{
    [SerializeField] private TerrainState terrain;
    [SerializeField] private ColliderRebuildScheduler colliderScheduler;
    [SerializeField] private RoomBuilder roomBuilder;

    private readonly List<PushableBlock> pending = new();
    private bool pendingRebuild;

    public void Enqueue(PushableBlock block)
    {
        Debug.Log($"[Settlement] Enqueue {block.name}");

        if (!pending.Contains(block))
            pending.Add(block);
    }

    private void LateUpdate()
    {
        if (pending.Count == 0)
            return;

        Debug.Log($"[Settlement] LateUpdate pending={pending.Count}");

        foreach (var block in pending)
            TrySettle(block);

        pending.Clear();

        if (pendingRebuild)
        {
            Debug.Log("[Settlement] Request collider rebuild");
            colliderScheduler.RequestRebuild();
            pendingRebuild = false;
        }
    }

    private void TrySettle(PushableBlock block)
    {
        Debug.Log($"[Settlement] TrySettle start {block.name}");

        if (terrain == null)
        {
            Debug.LogError(
                "[Settlement] TerrainState is null",
                this
            );
            return;
        }

        if (roomBuilder == null)
        {
            Debug.LogError(
                "[Settlement] RoomBuilder is null",
                this
            );
            return;
        }

        if (block.IsSettled)
        {
            Debug.Log("[Settlement] Å® already settled");
            return;
        }

        Rigidbody rb = block.GetComponent<Rigidbody>();
        Debug.Log($"[Settlement] velocity={rb.velocity}");

        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            Debug.Log("[Settlement] Å® still moving");
            return;
        }

        Vector3 localPos = block.transform.localPosition;
        Debug.Log($"[Settlement] localPos={localPos}");

        if (!terrain.TryFillFromCarryBlock(localPos, out Vector3 snapped))
        {
            Debug.Log("[Settlement] Å® TryFillFromCarryBlock FAILED");
            return;
        }

        float dist = (localPos - snapped).sqrMagnitude;
        Debug.Log($"[Settlement] center diff sqr={dist}");

        if (dist > 0.0625f)
        {
            Debug.Log("[Settlement] Å® center too far");
            return;
        }

        Debug.Log("[Settlement] Åö SETTLED");

        block.transform.localPosition = snapped;
        block.Freeze();
        block.MarkSettled();

        Instantiate(
            BlockFactory.GetPrefab('1'),
            roomBuilder.ContentRoot
        ).transform.localPosition = snapped;

        pendingRebuild = true;
    }

    // ================================
    // Dependency Injection
    // ================================

    public void SetTerrain(TerrainState terrain)
    {
        Debug.Log("[Settlement] SetTerrain");
        this.terrain = terrain;
    }

    public void SetRoomBuilder(RoomBuilder builder)
    {
        Debug.Log(
            $"[Settlement] SetRoomBuilder: {builder.name}",
            this
        );
        roomBuilder = builder;
    }
}
