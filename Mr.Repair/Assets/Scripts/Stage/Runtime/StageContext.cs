using UnityEngine;

/// <summary>
/// ステージを構成する各サブシステムを束ねるコンテキスト
/// 「生成」や「ロジック」は持たず、配線のみを担当する
/// </summary>
public class StageContext : MonoBehaviour
{
    private SettlementCoordinator settlementCoordinator;
    private ColliderRebuildScheduler colliderRebuildScheduler;
    private RoomBuilder roomBuilder;

    public TerrainState Terrain { get; private set; }
    public RoomBuilder RoomBuilder => roomBuilder;

    public void SetSettlementCoordinator(SettlementCoordinator coordinator)
    {
        settlementCoordinator = coordinator;
        coordinator.SetContext(this);
    }

    public void SetColliderRebuildScheduler(ColliderRebuildScheduler scheduler)
    {
        colliderRebuildScheduler = scheduler;
    }

    public void SetRoomBuilder(RoomBuilder builder)
    {
        roomBuilder = builder;
    }

    public void SetTerrain(TerrainState terrain)
    {
        Terrain = terrain;
    }

    public void RequestColliderRebuild()
    {
        colliderRebuildScheduler?.RequestRebuild();
    }
}

