using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResettableStageController : MonoBehaviour
{
    public static ResettableStageController Instance { get; private set; }

    public Transform player;
    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    private List<GameObject> initialPrefabs = new();
    private List<(Vector3 pos, Quaternion rot)> blockStartTransforms = new();

    public RoomBuilder roomBuilder;

    private void Awake()
    {
        Instance = this;

        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        // ★ 最初のシーンにあるブロックのみ登録
        foreach (var block in FindObjectsOfType<PushableBlock>())
        {
#if UNITY_EDITOR
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(block.gameObject);
            initialPrefabs.Add(prefab != null ? prefab : block.prefabReference);
#else
            initialPrefabs.Add(block.prefabReference);
#endif
            blockStartTransforms.Add((block.transform.position, block.transform.rotation));
        }
    }

    public void ResetStage()
    {
        foreach (var b in FindObjectsOfType<PushableBlock>())
            Destroy(b.gameObject);

        roomBuilder.BuildRoom();

        for (int i = 0; i < initialPrefabs.Count; i++)
        {
            var tf = blockStartTransforms[i];
            Instantiate(initialPrefabs[i], tf.pos, tf.rot, roomBuilder.ContentRoot);
        }

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
