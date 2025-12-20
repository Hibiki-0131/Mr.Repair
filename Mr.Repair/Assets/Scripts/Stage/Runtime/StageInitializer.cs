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
        // 1. Terrain 構築
        // ============================
        TerrainState terrain = roomBuilder.BuildTerrain();

        // StageContext に配線
        stageContext.SetTerrain(terrain);

        // ============================
        // 2. SettlementCoordinator 取得
        // ============================
        var settlement = GetComponent<SettlementCoordinator>();
        if (settlement == null)
        {
            Debug.LogError(
                "[StageInitializer] SettlementCoordinator not found",
                this
            );
            return;
        }

        // ★ Terrain / RoomBuilder を明示的に注入
        settlement.SetTerrain(terrain);
        settlement.SetRoomBuilder(roomBuilder);

        // ============================
        // 3. CarryBlock 生成
        // ============================
        GameObject carryPrefab = BlockFactory.GetPrefab('3');
        if (carryPrefab == null)
        {
            Debug.LogError(
                "[StageInitializer] CarryBlock prefab not found",
                this
            );
            return;
        }

        foreach (Vector3 pos in roomBuilder.GetCarryBlockPositions())
        {
            var block = Instantiate(
                carryPrefab,
                pos,
                Quaternion.identity,
                roomBuilder.ContentRoot
            );

            // ============================
            // 4. BlockSettlementSensor 配線
            // ============================
            var sensor = block.GetComponent<BlockSettlementSensor>();
            if (sensor == null)
            {
                Debug.LogError(
                    "[StageInitializer] BlockSettlementSensor missing on CarryBlock",
                    block
                );
                continue;
            }

            sensor.SetCoordinator(settlement);
        }

        Debug.Log("[StageInitializer] InitializeStage completed", this);
    }
}
