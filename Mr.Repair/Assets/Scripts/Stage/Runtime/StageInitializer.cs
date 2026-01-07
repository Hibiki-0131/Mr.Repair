using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RoomBuilder roomBuilder;
    [SerializeField] private StageContext stageContext;

    private bool initialized;

    public void SetDependencies(
        RoomBuilder builder,
        StageContext context
    )
    {
        roomBuilder = builder;
        stageContext = context;
    }

    private void Awake()
    {
        if (roomBuilder == null)
            roomBuilder = GetComponent<RoomBuilder>();

        if (stageContext == null)
            stageContext = GetComponent<StageContext>();
    }

    private void Start()
    {
        if (initialized)
            return;

        if (roomBuilder == null || stageContext == null)
        {
            Debug.LogError(
                "[StageInitializer] Dependencies not set",
                this
            );
            return;
        }

        InitializeStage();
        initialized = true;
    }

    private void InitializeStage()
    {
        // ============================
        // 1. Terrain ç\íz
        // ============================
        TerrainState terrain = roomBuilder.BuildTerrain();

        // ============================
        // 2. Context Ç…èWñÒ
        // ============================
        stageContext.SetTerrain(terrain);
        stageContext.SetRoomBuilder(roomBuilder);

        var settlement = GetComponent<SettlementCoordinator>();
        if (settlement == null)
        {
            Debug.LogError(
                "[StageInitializer] SettlementCoordinator not found",
                this
            );
            return;
        }

        stageContext.SetSettlementCoordinator(settlement);

        // Åö Coordinator Ç…ÇÕ Context ÇæÇØìnÇ∑
        settlement.SetContext(stageContext);

        // ============================
        // 3. CarryBlock ê∂ê¨ÇÕ RoomBuilder Ç…àœè˜
        // ============================
        GameObject carryPrefab = BlockFactory.GetPrefab('3');
        roomBuilder.SpawnInitialCarryBlocks(
            carryPrefab,
            settlement
        );

        Debug.Log("[StageInitializer] InitializeStage completed", this);
    }

}
