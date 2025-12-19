using System.Collections.Generic;
using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    public static ResettableStageController Instance { get; private set; }

    [Header("Player")]
    public Transform player;
    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    [Header("Room")]
    public RoomBuilder roomBuilder;

    // ================================
    // 初期 CarryBlock 情報
    // ================================
    private readonly List<(Vector3 localPos, Quaternion localRot)> blockStartTransforms
        = new();

    private void Awake()
    {
        Instance = this;

        // ----------------
        // Player 初期位置
        // ----------------
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        // ----------------
        // 初期 CarryBlock を記録
        // （Prefabは保持しない）
        // ----------------
        foreach (var block in FindObjectsOfType<PushableBlock>())
        {
            blockStartTransforms.Add((
                block.transform.localPosition,
                block.transform.localRotation
            ));
        }
    }

    /// <summary>
    /// ステージを CSV 初期状態に完全リセット
    /// </summary>
    public void ResetStage()
    {
        if (roomBuilder == null)
        {
            Debug.LogError("[ResettableStageController] RoomBuilder is NULL");
            return;
        }

        // ================================
        // 1. 既存 CarryBlock を削除
        // ================================
        foreach (var b in FindObjectsOfType<PushableBlock>())
        {
            Destroy(b.gameObject);
        }

        // ================================
        // 2. Room を CSV から再構築
        //    （床・壁・Collider を含む）
        // ================================
        roomBuilder.BuildRoom();

        // ================================
        // 3. CarryBlock を再生成
        // ================================
        GameObject carryPrefab = BlockFactory.GetPrefab('3');
        if (carryPrefab == null)
        {
            Debug.LogError(
                "[ResettableStageController] CarryBlock prefab not found (csv=3)"
            );
            return;
        }

        foreach (var data in blockStartTransforms)
        {
            var block = Instantiate(
                carryPrefab,
                roomBuilder.ContentRoot
            );

            block.transform.localPosition = data.localPos;
            block.transform.localRotation = data.localRot;

            var pushable = block.GetComponent<PushableBlock>();
            if (pushable != null)
            {
                pushable.SetOwner(roomBuilder);
            }
        }

        // ================================
        // 4. Player を初期位置に戻す
        // ================================
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = playerStartPos;
            playerRb.rotation = playerStartRot;
        }

        Physics.SyncTransforms();

        Debug.Log("[ResettableStageController] ResetStage completed");
    }
}
