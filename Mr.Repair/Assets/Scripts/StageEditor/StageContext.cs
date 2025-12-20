using UnityEngine;

public class StageContext : MonoBehaviour
{
    [SerializeField] private SettlementCoordinator settlementCoordinator;
    [SerializeField] private ColliderRebuildScheduler colliderRebuildScheduler;

    public TerrainState Terrain { get; private set; }

    /// <summary>
    /// TerrainState を各サブシステムへ配線
    /// </summary>
    public void SetTerrain(TerrainState terrain)
    {
        Terrain = terrain;

        if (settlementCoordinator != null)
            settlementCoordinator.SetTerrain(terrain);

        if (colliderRebuildScheduler != null)
            colliderRebuildScheduler.SetTerrain(terrain);
    }
}
