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
    }

    private void CreateRoomInScene()
    {
        // ----------------------------
        // Prefab Instantiate
        // ----------------------------
        var room = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab);
        if (room == null)
        {
            Debug.LogError("Prefab の Instantiate に失敗しました");
            return;
        }

        Undo.RegisterCreatedObjectUndo(room, "Create Room");

        room.name = selectedMetadata.roomName;
        room.transform.position = spawnPosition;
        room.transform.rotation = Quaternion.identity;
        room.transform.localScale = Vector3.one;

        // ----------------------------
        // 必須コンポーネント（RoomRoot 直下）
        // ----------------------------
        var builder =
            room.GetComponent<RoomBuilder>() ??
            room.AddComponent<RoomBuilder>();

        var context =
            room.GetComponent<StageContext>() ??
            room.AddComponent<StageContext>();

        var initializer =
            room.GetComponent<StageInitializer>() ??
            room.AddComponent<StageInitializer>();

        var resetController =
            room.GetComponent<ResettableStageController>() ??
            room.AddComponent<ResettableStageController>();

        var settlementCoordinator =
            room.GetComponent<SettlementCoordinator>() ??
            room.AddComponent<SettlementCoordinator>();

        var colliderScheduler =
            room.GetComponent<ColliderRebuildScheduler>() ??
            room.AddComponent<ColliderRebuildScheduler>();

        // ----------------------------
        // RoomMetadataHolder
        // ----------------------------
        var holder = room.GetComponentInChildren<RoomMetadataHolder>();
        if (holder == null)
        {
            var holderGO = new GameObject("RoomMetadataHolder");
            holderGO.transform.SetParent(room.transform);
            holderGO.transform.localPosition = Vector3.zero;
            holderGO.transform.localRotation = Quaternion.identity;
            holderGO.transform.localScale = Vector3.one;

            holder = holderGO.AddComponent<RoomMetadataHolder>();
        }
        holder.metadata = selectedMetadata;

        // ----------------------------
        // ContentRoot（★必ず 1 つだけ）
        // ----------------------------
        if (builder.ContentRoot == null)
        {
            // 既存 ContentRoot を探索
            Transform existing = room.transform.Find("ContentRoot");

            if (existing != null)
            {
                builder.SetContentRoot(existing);
            }
            else
            {
                var contentRootGO = new GameObject("ContentRoot");
                contentRootGO.transform.SetParent(room.transform);
                contentRootGO.transform.localPosition = Vector3.zero;
                contentRootGO.transform.localRotation = Quaternion.identity;
                contentRootGO.transform.localScale = Vector3.one;

                builder.SetContentRoot(contentRootGO.transform);
            }
        }

        // ----------------------------
        // 相互参照の自動配線
        // ----------------------------
        initializer.SetDependencies(builder, context);
        resetController.SetDependencies(builder, context, settlementCoordinator);

        context.SetSettlementCoordinator(settlementCoordinator);
        context.SetColliderRebuildScheduler(colliderScheduler);

        colliderScheduler.SetOwner(builder);

        // ----------------------------
        // Editor 用ビルド（※ carryblock は生成しない）
        // ----------------------------
        builder.BuildForEditor(selectedMetadata);

        // ----------------------------
        // Selection
        // ----------------------------
        Selection.activeGameObject = room;
        EditorGUIUtility.PingObject(room);
    }
}
