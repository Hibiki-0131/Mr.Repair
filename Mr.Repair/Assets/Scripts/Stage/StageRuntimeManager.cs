using UnityEngine;
using System.Collections.Generic;

public class StageRuntimeManager : MonoBehaviour
{
    public static StageRuntimeManager Instance { get; private set; }

    private readonly List<ResettableStageController> rooms = new();

    private Rigidbody playerRb;
    private Vector3 playerStartPos;
    private Quaternion playerStartRot;

    private bool startPositionRecorded; // Åö èâä˙à íuÇ™ämíËÇµÇΩÇ©

    // ================================
    // Singleton
    // ================================
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ================================
    // ñ≥ÇØÇÍÇŒê∂ê¨
    // ================================
    public static StageRuntimeManager EnsureExists()
    {
        if (Instance != null)
            return Instance;

        var go = new GameObject("StageRuntimeManager");
        return go.AddComponent<StageRuntimeManager>();
    }

    // ================================
    // Roomé©ìÆìoò^
    // ================================
    public void RegisterRoom(ResettableStageController room)
    {
        if (room == null)
            return;

        if (rooms.Contains(room))
            return;

        rooms.Add(room);
    }

    // ================================
    // ? Playerìoò^Åièâä˙à íuämíËÅj
    // ================================
    public void RegisterPlayer(Transform player)
    {
        if (player == null)
            return;

        playerRb = player.GetComponent<Rigidbody>();

        if (playerRb == null)
        {
            Debug.LogWarning("[StageRuntimeManager] Player Rigidbody missing");
            return;
        }

        // Åö èââÒÇÃÇ›ãLò^
        if (!startPositionRecorded)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            startPositionRecorded = true;
        }
    }

    // ================================
    // Stage Reset
    // ================================
    public void ResetStage()
    {
        foreach (var room in rooms)
        {
            if (room != null)
                room.ResetRoomInternal();
        }

        ResetPlayer();
    }

    // ================================
    // Player Reset
    // ================================
    private void ResetPlayer()
    {
        if (!startPositionRecorded || playerRb == null)
            return;

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        playerRb.position = playerStartPos;
        playerRb.rotation = playerStartRot;

        Physics.SyncTransforms();
    }
}
