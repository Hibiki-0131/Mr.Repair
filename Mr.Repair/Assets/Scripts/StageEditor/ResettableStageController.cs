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
    private List<GameObject> initialBlockPrefabs = new();
    private List<Vector3> blockStartPos = new();
    private List<Quaternion> blockStartRot = new();

    [Header("RoomBuilder")]
    public RoomBuilder roomBuilder;

    private void Awake()
    {
        Instance = this;

        // Player 情報
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        // CarryBlock の初期状態記録
        foreach (var block in FindObjectsOfType<PushableBlock>())
        {
            var go = block.gameObject;

            // ★ Prefab を保存
            var prefab = block.prefabReference; // あなたの PushableBlock に追加する必要あり
            initialBlockPrefabs.Add(prefab);

            blockStartPos.Add(go.transform.position);
            blockStartRot.Add(go.transform.rotation);
        }
    }

    public void ResetStage()
    {
        Debug.Log("ステージリセット開始");

        // ① Room 再構築
        roomBuilder.BuildRoom();

        // ② CarryBlock 全削除
        foreach (var b in FindObjectsOfType<PushableBlock>())
            Destroy(b.gameObject);

        // ③ 初期位置に再生成
        for (int i = 0; i < initialBlockPrefabs.Count; i++)
        {
            Instantiate(initialBlockPrefabs[i], blockStartPos[i], blockStartRot[i]);
        }

        // ④ Player 再配置
        if (playerRb)
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

        // ⑤ Transform 反映
        Physics.SyncTransforms();

        Debug.Log("ステージリセット完了");
    }
}
