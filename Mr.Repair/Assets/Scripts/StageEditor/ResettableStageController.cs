using System.Collections.Generic;
using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RoomBuilder roomBuilder;
    [SerializeField] private StageContext stageContext;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    private readonly List<(Vector3 pos, Quaternion rot)> blockStarts = new();

    private void Awake()
    {
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        foreach (var b in FindObjectsOfType<PushableBlock>())
            blockStarts.Add((b.transform.localPosition, b.transform.localRotation));
    }

    public void ResetStage()
    {
        // -----------------------------
        // 既存 CarryBlock 削除
        // -----------------------------
        foreach (var b in FindObjectsOfType<PushableBlock>())
            Destroy(b.gameObject);

        // -----------------------------
        // Terrain 再構築
        // -----------------------------
        if (roomBuilder == null || stageContext == null)
        {
            Debug.LogError("[ResettableStageController] Missing references", this);
            return;
        }

        TerrainState terrain = roomBuilder.BuildTerrain();
        if (terrain == null)
        {
            Debug.LogError("[ResettableStageController] Terrain build failed", this);
            return;
        }

        stageContext.SetTerrain(terrain);

        // -----------------------------
        // CarryBlock 再生成
        // -----------------------------
        GameObject carryPrefab = BlockFactory.GetPrefab('3');
        if (carryPrefab == null)
        {
            Debug.LogError(
                "[ResettableStageController] CarryBlock prefab not found in BlockFactory",
                this
            );
            return;
        }

        foreach (var t in blockStarts)
        {
            var b = Instantiate(carryPrefab, roomBuilder.ContentRoot);
            b.transform.localPosition = t.pos;
            b.transform.localRotation = t.rot;
        }

        // -----------------------------
        // Player リセット
        // -----------------------------
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
