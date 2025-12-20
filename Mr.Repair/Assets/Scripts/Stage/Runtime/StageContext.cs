using UnityEngine;

/// <summary>
/// ステージを構成する各サブシステムを束ねるコンテキスト
/// 「生成」や「ロジック」は持たず、配線のみを担当する
/// </summary>
public class StageContext : MonoBehaviour
{
    private SettlementCoordinator settlementCoordinator;
    private ColliderRebuildScheduler colliderRebuildScheduler;

    public TerrainState Terrain { get; private set; }

    // ================================
    // 配線 API（Editor / Runtime 共通）
    // ================================

    public void SetSettlementCoordinator(SettlementCoordinator coordinator)
    {
        settlementCoordinator = coordinator;
    }

    public void SetColliderRebuildScheduler(ColliderRebuildScheduler scheduler)
    {
        colliderRebuildScheduler = scheduler;
    }

    // ================================
    // Terrain 配線
    // ================================

    public void SetTerrain(TerrainState terrain)
    {
        Terrain = terrain;

        if (settlementCoordinator != null)
            settlementCoordinator.SetTerrain(terrain);

        if (colliderRebuildScheduler != null)
            colliderRebuildScheduler.SetTerrain(terrain);
    }
}
