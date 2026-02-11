using UnityEngine;
using System.Collections;
using System.Linq;

public class ResettableStageController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private RoomBuilder roomBuilder;
    private StageContext stageContext;
    private SettlementCoordinator settlementCoordinator;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    private bool isResetting;

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

    private void ResolveDependencies()
    {
        if (roomBuilder == null)
            roomBuilder = GetComponent<RoomBuilder>();

        if (stageContext == null)
            stageContext = GetComponent<StageContext>();

        if (settlementCoordinator == null)
            settlementCoordinator = GetComponent<SettlementCoordinator>();
    }

    // ================================
    // ★ 外部公開（RuntimeManager用）
    // ================================
    public void ResetRoomInternal()
    {
        if (isResetting) return;

        StartCoroutine(ResetRoutine());
    }

    // ================================
    private IEnumerator ResetRoutine()
    {
        isResetting = true;

        ResolveDependencies();

        if (roomBuilder == null ||
            stageContext == null ||
            settlementCoordinator == null)
        {
            Debug.LogError("[ResettableStageController] Dependencies not set");
            isResetting = false;
            yield break;
        }

        settlementCoordinator.enabled = false;

        // -------------------------
        // PushableBlock削除
        // -------------------------
        foreach (var block in FindObjectsOfType<PushableBlock>())
            Destroy(block.gameObject);

        yield return null;
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        // -------------------------
        // Terrain再構築
        // -------------------------
        TerrainState newTerrain = roomBuilder.BuildTerrain();

        stageContext.SetRoomBuilder(roomBuilder);
        stageContext.SetSettlementCoordinator(settlementCoordinator);
        stageContext.SetColliderRebuildScheduler(
            roomBuilder.GetComponent<ColliderRebuildScheduler>()
        );
        stageContext.SetTerrain(newTerrain);

        // -------------------------
        // CarryBlock再生成
        // -------------------------
        GameObject carryPrefab = BlockFactory.GetPrefab('3');

        if (carryPrefab != null)
        {
            roomBuilder.SpawnInitialCarryBlocks(
                carryPrefab,
                settlementCoordinator
            );
        }

        // -------------------------
        // Playerリセット
        // -------------------------
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = playerStartPos;
            playerRb.rotation = playerStartRot;
        }

        Physics.SyncTransforms();

        settlementCoordinator.enabled = true;

        isResetting = false;
    }
}
