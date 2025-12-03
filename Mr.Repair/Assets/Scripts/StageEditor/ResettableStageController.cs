using System.Collections.Generic;
using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    public static ResettableStageController Instance { get; private set; }

    [Header("Player 初期情報")]
    public Transform player;
    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    [Header("CarryBlock 初期情報")]
    private readonly List<GameObject> initialBlockPrefabs = new();
    private readonly List<Vector3> initialPositions = new();
    private readonly List<Quaternion> initialRotations = new();

    [Header("RoomBuilder")]
    public RoomBuilder roomBuilder;

    private void Awake()
    {
        Instance = this;

        // ▼ Player 初期情報保存
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        // ▼ CarryBlock 初期情報保存
        foreach (var block in FindObjectsOfType<PushableBlock>())
        {
            initialBlockPrefabs.Add(block.prefabReference);
            initialPositions.Add(block.transform.position);
            initialRotations.Add(block.transform.rotation);
        }
    }

    // ============================================================
    // ▼ Reset 実行
    // ============================================================
    public void ResetStage()
    {
        Debug.Log("=== Reset Stage Begin ===");

        // ① CarryBlock を全削除（RoomBuilder 再構築前にやる）
        foreach (var b in FindObjectsOfType<PushableBlock>())
        {
            Destroy(b.gameObject);
        }

        // ② Room（地形）を再構築
        roomBuilder.BuildRoom();

        // ③ CarryBlock を初期状態で再生成（親は null → Scene Root）
        for (int i = 0; i < initialBlockPrefabs.Count; i++)
        {
            Instantiate(
                initialBlockPrefabs[i],
                initialPositions[i],
                initialRotations[i],
                null
            );
        }

        // ④ Player を初期状態に戻す
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = playerStartPos;
            playerRb.rotation = playerStartRot;
        }
        else
        {
            player.position = playerStartPos;
            player.rotation = playerStartRot;
        }

        Physics.SyncTransforms();

        Debug.Log("=== Reset Stage Complete ===");
    }
}
