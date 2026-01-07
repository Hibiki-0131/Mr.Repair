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
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        ResolveDependencies();
    }

    // ================================
    // Dependency Resolution
    // ================================
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
        // 2. Terrain 再構築
        // -------------------------
        TerrainState newTerrain = roomBuilder.BuildTerrain();

        // -------------------------
        // 3. StageContext 再配線（★重要）
        // -------------------------
        stageContext.SetRoomBuilder(roomBuilder);
        stageContext.SetSettlementCoordinator(settlementCoordinator);
        stageContext.SetColliderRebuildScheduler(
            roomBuilder.GetComponent<ColliderRebuildScheduler>()
        );

        stageContext.SetTerrain(newTerrain);

        // -------------------------
        // 4. CarryBlock 再生成
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
