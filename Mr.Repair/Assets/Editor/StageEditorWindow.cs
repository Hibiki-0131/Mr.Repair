using UnityEditor;
using UnityEngine;

public class StageEditorWindow : EditorWindow
{
    private RoomMetadata selectedMetadata;
    private GameObject roomPrefab;
    private Vector3 spawnPosition = Vector3.zero;

    [MenuItem("Tools/Stage Editor")]
    public static void Open()
    {
        GetWindow<StageEditorWindow>("Stage Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Placement", EditorStyles.boldLabel);

        roomPrefab = (GameObject)EditorGUILayout.ObjectField(
            "RoomRoot Prefab",
            roomPrefab,
            typeof(GameObject),
            false
        );

        selectedMetadata = (RoomMetadata)EditorGUILayout.ObjectField(
            "Room Metadata",
            selectedMetadata,
            typeof(RoomMetadata),
            false
        );

        spawnPosition = EditorGUILayout.Vector3Field(
            "Spawn Position",
            spawnPosition
        );

        GUILayout.Space(10);

        using (new EditorGUI.DisabledScope(roomPrefab == null || selectedMetadata == null))
        {
            if (GUILayout.Button("Create Room in Scene"))
            {
                CreateRoomInScene();
            }
        }

        GUILayout.Space(15);
        GUILayout.Label("CSV Preview", EditorStyles.boldLabel);

        if (selectedMetadata != null && selectedMetadata.roomCsv != null)
        {
            EditorGUILayout.TextArea(
                selectedMetadata.roomCsv.text,
                GUILayout.Height(200)
            );
        }
    }

    private void CreateRoomInScene()
    {
        if (roomPrefab == null || selectedMetadata == null)
        {
            Debug.LogWarning("Prefab Ç‹ÇΩÇÕ Metadata Ç™ñ¢ê›íËÇ≈Ç∑");
            return;
        }

        // ----------------------------
        // Prefab Instantiate
        // ----------------------------
        var room = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab);
        if (room == null)
        {
            Debug.LogError("Prefab ÇÃ Instantiate Ç…é∏îsÇµÇ‹ÇµÇΩ");
            return;
        }

        Undo.RegisterCreatedObjectUndo(room, "Create Room");

        room.name = selectedMetadata.roomName;

        // ----------------------------
        // Transform èâä˙âª
        // ----------------------------
        room.transform.position = spawnPosition;
        room.transform.rotation = Quaternion.identity;
        room.transform.localScale = Vector3.one;

        // ----------------------------
        // Metadata ê›íË
        // ----------------------------
        var holder = room.GetComponentInChildren<RoomMetadataHolder>();
        if (holder == null)
        {
            Debug.LogError("RoomMetadataHolder Ç™ RoomPrefab ì‡Ç…å©Ç¬Ç©ÇËÇ‹ÇπÇÒ");
            return;
        }
        holder.metadata = selectedMetadata;

        // ----------------------------
        // RoomBuilder éÊìæ
        // ----------------------------
        var builder = room.GetComponentInChildren<RoomBuilder>();
        if (builder == null)
        {
            Debug.LogError("RoomBuilder Ç™ RoomPrefab ì‡Ç…å©Ç¬Ç©ÇËÇ‹ÇπÇÒ");
            return;
        }

        // ----------------------------
        // contentRoot ê≥ãKâª
        // ----------------------------
        Transform contentRoot = builder.ContentRoot;
        if (contentRoot != null)
        {
            contentRoot.localPosition = Vector3.zero;
            contentRoot.localRotation = Quaternion.identity;
            contentRoot.localScale = Vector3.one;
        }

        // ----------------------------
        // Åö Editor êÍóp API Çégóp
        // ----------------------------
        builder.BuildForEditor(selectedMetadata);

        // ----------------------------
        // Selection
        // ----------------------------
        Selection.activeGameObject = room;
        EditorGUIUtility.PingObject(room);
    }
}
