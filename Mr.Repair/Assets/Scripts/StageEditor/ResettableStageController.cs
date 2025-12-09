using System.Collections.Generic;
using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    public static ResettableStageController Instance { get; private set; }

    public Transform player;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    public RoomBuilder roomBuilder;

    private readonly List<PushableBlock> blocks = new();
    private readonly List<Vector3> initPos = new();
    private readonly List<Quaternion> initRot = new();

    private void Awake()
    {
        Instance = this;

        // Player‰Šúî•ñ
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }

        // carryblock‰Šúî•ñ
        foreach (var block in FindObjectsOfType<PushableBlock>())
        {
            blocks.Add(block);
            initPos.Add(block.transform.position);
            initRot.Add(block.transform.rotation);
        }
    }

    public void ResetStage()
    {
        Debug.Log("=== Reset Stage Begin ===");

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].ResetBlock(initPos[i], initRot[i]);
        }

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.position = playerStartPos;
        playerRb.rotation = playerStartRot;

        Physics.SyncTransforms();
        Debug.Log("=== Reset Stage Complete ===");
    }
}
