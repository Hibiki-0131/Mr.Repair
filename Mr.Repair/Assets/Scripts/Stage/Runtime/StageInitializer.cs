using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RoomBuilder roomBuilder;
    [SerializeField] private StageContext stageContext;

    private bool initialized;

    public void SetDependencies(RoomBuilder builder, StageContext context)
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
            Debug.LogError("[StageInitializer] Dependencies not set", this);
            return;
        }

        InitializeStage();
        initialized = true;
    }

    private void InitializeStage()
    {
        TerrainState terrain = roomBuilder.BuildTerrain();

        stageContext.SetTerrain(terrain);
        stageContext.SetRoomBuilder(roomBuilder);

        var settlement = GetComponent<SettlementCoordinator>();
        stageContext.SetSettlementCoordinator(settlement);
        settlement.SetContext(stageContext);

        GameObject carryPrefab = BlockFactory.GetPrefab('3');

        if (carryPrefab != null)
        {
            roomBuilder.SpawnInitialCarryBlocks(carryPrefab, settlement);
        }

        Debug.Log("[StageInitializer] InitializeStage completed", this);
    }
}
