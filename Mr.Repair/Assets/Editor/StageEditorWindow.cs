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

        if (GUILayout.Button("Create Room in Scene"))
        {
            CreateRoomInScene();
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
            Debug.LogWarning("Prefab Ç‹ÇΩÇÕ Metadata Ç™ñ¢ê›íËÇ≈Ç∑");
            return;
        }

        GameObject room =
            (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab);

        room.name = selectedMetadata.roomName;
        room.transform.position = spawnPosition;
        room.transform.rotation = Quaternion.identity;

        var holder = room.GetComponentInChildren<RoomMetadataHolder>();
        holder.metadata = selectedMetadata;

        var builder = room.GetComponentInChildren<RoomBuilder>();
        builder.BuildRoom();

        Selection.activeGameObject = room;
    }
}
