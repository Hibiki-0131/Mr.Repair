using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private RoomBuilder roomBuilder;
    private StageContext stageContext;
    private SettlementCoordinator settlementCoordinator;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    // ================================
    // Unity Lifecycle
    // ================================
    private void Awake()
    {
        // プレイヤー初期状態保存
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        ResolveDependencies();
    }

    // ================================
    // Dependency Injection
    // ================================
    public void SetDependencies(
        RoomBuilder builder,
        StageContext context,
        SettlementCoordinator settlement
    )
    {
        roomBuilder = builder;
        stageContext = context;
        settlementCoordinator = settlement;
    }

    private void ResolveDependencies()
    {
        if (roomBuilder == null)
            roomBuilder = FindObjectOfType<RoomBuilder>();

        if (stageContext == null)
            stageContext = FindObjectOfType<StageContext>();

        if (settlementCoordinator == null)
            settlementCoordinator = FindObjectOfType<SettlementCoordinator>();
    }

    // ================================
    // Reset API
    // ================================
    public void ResetStage()
    {
        ResolveDependencies();

        if (roomBuilder == null ||
            stageContext == null ||
            settlementCoordinator == null)
        {
            Debug.LogError(
                "[ResettableStageController] Dependencies not set",
                this
            );
            return;
        }

        // -------------------------
        // 1. 動的エンティティ破棄
        // -------------------------
        foreach (var block in FindObjectsOfType<PushableBlock>())
        {
            Destroy(block.gameObject);
        }

        // -------------------------
        // 2. Terrain 再構築（CSV起点）
        // -------------------------
        TerrainState newTerrain = roomBuilder.BuildTerrain();

        // -------------------------
        // 3. Context 再注入
        // -------------------------
        stageContext.SetTerrain(newTerrain);

        // -------------------------
        // 4. CarryBlock 再生成（★追加）
        // -------------------------
        GameObject carryPrefab = BlockFactory.GetPrefab('3');
        roomBuilder.SpawnInitialCarryBlocks(
            carryPrefab,
            settlementCoordinator
        );

        // -------------------------
        // 5. Player リセット
        // -------------------------
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = playerStartPos;
            playerRb.rotation = playerStartRot;
        }

        Physics.SyncTransforms();
    }
}
