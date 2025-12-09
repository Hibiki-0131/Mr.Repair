using System.Collections.Generic;
using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    public static ResettableStageController Instance { get; private set; }

    [Header("Player 初期位置")]
    public Transform player;
    private Vector3 playerStartPos;
    private Quaternion playerStartRot;

    [Header("CarryBlock 初期情報")]
    private List<PushableBlock> initialBlocks = new();
    private List<(Vector3 pos, Quaternion rot)> blockStartTransforms = new();

    [Header("RoomBuilder")]
    public RoomBuilder roomBuilder;

    public Transform ContentRoot => contentRoot;

    private void Awake()
    {
        Instance = this;

        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
        }
    }

    // PushableBlock.Start() から呼ばれる
    public void RegisterCarryBlock(PushableBlock block)
    {
        if (!initialBlocks.Contains(block))
        {
            initialBlocks.Add(block);
            blockStartTransforms.Add((block.transform.position, block.transform.rotation));
        }
    }

    public void ResetStage()
    {
        Debug.Log("ステージリセット開始");

        // 地形を CSV 初期状態に戻す
        roomBuilder.BuildRoom();

        // 既存のブロック削除
        foreach (var b in FindObjectsOfType<PushableBlock>())
            Destroy(b.gameObject);

        // 初期位置復元
        for (int i = 0; i < initialBlocks.Count; i++)
        {
            var prefab = initialBlocks[i].prefabReference;
            var tf = blockStartTransforms[i];
            Instantiate(prefab, tf.pos, tf.rot, roomBuilder.ContentRoot);
        }

        // Player戻す
        if (player != null)
        {
            player.position = playerStartPos;
            player.rotation = playerStartRot;
        }

        Debug.Log("ステージリセット完了");
    }
}
