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
            "RoomRoot Prefab", roomPrefab, typeof(GameObject), false);

        selectedMetadata = (RoomMetadata)EditorGUILayout.ObjectField(
            "Room Metadata", selectedMetadata, typeof(RoomMetadata), false);

        spawnPosition = EditorGUILayout.Vector3Field(
            "Spawn Position", spawnPosition);

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
                selectedMetadata.roomCsv.text, GUILayout.Height(200));
        }
    }

    private void CreateRoomInScene()
    {
        if (roomPrefab == null || selectedMetadata == null)
        {
            Debug.LogWarning("Prefab または Metadata が未設定です");
            return;
        }

        var room = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab);
        if (room == null)
        {
            Debug.LogError("Prefab の Instantiate に失敗しました");
            return;
        }

        Undo.RegisterCreatedObjectUndo(room, "Create Room");

        room.name = selectedMetadata.roomName;

        // ★ RoomRoot を SpawnPosition に配置
        room.transform.position = spawnPosition;
        room.transform.rotation = Quaternion.identity;
        room.transform.localScale = Vector3.one;

        var holder = room.GetComponentInChildren<RoomMetadataHolder>();
        if (holder == null)
        {
            Debug.LogError("RoomMetadataHolder が RoomPrefab 内に見つかりません");
            return;
        }
        holder.metadata = selectedMetadata;

        var builder = room.GetComponentInChildren<RoomBuilder>();
        if (builder == null)
        {
            Debug.LogError("RoomBuilder が RoomPrefab 内に見つかりません");
            return;
        }

        // ★ contentRoot を必ずローカル原点に揃える
        Transform contentRoot = builder.ContentRoot;
        if (contentRoot != null)
        {
            contentRoot.localPosition = Vector3.zero;
            contentRoot.localRotation = Quaternion.identity;
            contentRoot.localScale = Vector3.one;
        }

        // ★ Editor 生成時は明示的に Build
        builder.BuildRoom();

        Selection.activeGameObject = room;
        EditorGUIUtility.PingObject(room);
    }
}
