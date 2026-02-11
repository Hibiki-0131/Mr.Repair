using UnityEngine;
using System.Collections.Generic;

public class StageRuntimeManager : MonoBehaviour
{
    public static StageRuntimeManager Instance { get; private set; }

    private readonly List<ResettableStageController> rooms = new();

    private void Awake()
    {
        // Åö Singletonï€èÿ
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        RegisterExistingRooms();
    }

    // Åö ñ≥ÇØÇÍÇŒé©ìÆê∂ê¨
    public static StageRuntimeManager EnsureExists()
    {
        if (Instance != null)
            return Instance;

        var go = new GameObject("StageRuntimeManager");
        return go.AddComponent<StageRuntimeManager>();
    }

    private void RegisterExistingRooms()
    {
        rooms.Clear();
        rooms.AddRange(FindObjectsOfType<ResettableStageController>());
    }

    public void ResetStage()
    {
        foreach (var room in rooms)
            room.ResetRoomInternal();
    }
}
