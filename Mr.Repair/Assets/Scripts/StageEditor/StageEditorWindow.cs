using UnityEditor;
using UnityEngine;

public class StageEditorWindow : EditorWindow
{
    private RoomMetadata selectedMetadata;
    private GameObject roomPrefab;

    [MenuItem("Tools/Stage Editor")]
    public static void Open()
    {
        GetWindow<StageEditorWindow>("Stage Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Placement", EditorStyles.boldLabel);

        roomPrefab = (GameObject)EditorGUILayout.ObjectField("RoomCube Prefab", roomPrefab, typeof(GameObject), false);
        selectedMetadata = (RoomMetadata)EditorGUILayout.ObjectField("Room Metadata", selectedMetadata, typeof(RoomMetadata), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create Room in Scene"))
        {
            CreateRoomInScene();
        }

        GUILayout.Space(15);
        GUILayout.Label("CSV Preview", EditorStyles.boldLabel);

        if (selectedMetadata != null && selectedMetadata.roomCsv != null)
        {
            string preview = selectedMetadata.roomCsv.text;
            EditorGUILayout.TextArea(preview, GUILayout.Height(200));
        }
    }

    private void CreateRoomInScene()
    {
        if (roomPrefab == null || selectedMetadata == null)
        {
            Debug.LogWarning("Prefab Ç‹ÇΩÇÕ Metadata Ç™ñ¢ê›íËÇ≈Ç∑");
            return;
        }

        GameObject room = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab);
        room.name = selectedMetadata.roomName;

        var holder = room.GetComponent<RoomMetadataHolder>();
        holder.metadata = selectedMetadata;

        var builder = room.GetComponent<RoomBuilder>();
        builder.BuildRoom();

        Selection.activeGameObject = room;
    }
}
