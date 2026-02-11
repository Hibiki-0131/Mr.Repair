using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RoomBuilder))]
[RequireComponent(typeof(StageContext))]
[RequireComponent(typeof(SettlementCoordinator))]
public class ResettableStageController : MonoBehaviour
{
    private RoomBuilder roomBuilder;
    private StageContext stageContext;
    private SettlementCoordinator settlementCoordinator;

    private bool isResetting;

    // ================================
    private void Awake()
    {
        ResolveDependencies();

        // ? RuntimeManagerへ登録
        StageRuntimeManager.EnsureExists()
            .RegisterRoom(this);
    }

    private void ResolveDependencies()
    {
        roomBuilder ??= GetComponent<RoomBuilder>();
        stageContext ??= GetComponent<StageContext>();
        settlementCoordinator ??= GetComponent<SettlementCoordinator>();
    }

    // ================================
    public void ResetRoomInternal()
    {
        if (isResetting) return;

        StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        isResetting = true;

        ResolveDependencies();

        if (roomBuilder == null ||
            stageContext == null ||
            settlementCoordinator == null)
        {
            Debug.LogError($"{name} Reset dependencies missing");
            isResetting = false;
            yield break;
        }

        settlementCoordinator.enabled = false;

        // =====================================================
        // ? ContentRoot配下のみ削除（超重要）
        // =====================================================
        if (roomBuilder.ContentRoot != null)
        {
            foreach (var block in roomBuilder.ContentRoot
                     .GetComponentsInChildren<PushableBlock>())
            {
                Destroy(block.gameObject);
            }
        }

        // Destroy同期待ち
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        // =====================================================
        // Terrain再構築
        // =====================================================
        TerrainState newTerrain = roomBuilder.BuildTerrain();

        stageContext.SetRoomBuilder(roomBuilder);
        stageContext.SetSettlementCoordinator(settlementCoordinator);

        // ? ColliderScheduler再接続
        stageContext.SetColliderRebuildScheduler(
            roomBuilder.GetComponent<ColliderRebuildScheduler>()
        );

        stageContext.SetTerrain(newTerrain);

        // =====================================================
        // CarryBlock再生成
        // =====================================================
        GameObject carryPrefab = BlockFactory.GetPrefab('3');

        if (carryPrefab != null)
        {
            roomBuilder.SpawnInitialCarryBlocks(
                carryPrefab,
                settlementCoordinator
            );
        }
        else
        {
            Debug.LogError("Carry prefab missing");
        }

        settlementCoordinator.enabled = true;

        isResetting = false;
    }
}
