using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomBuilder : MonoBehaviour
{
    // ================================
    // Settings
    // ================================
    [Header("Room Settings")]
    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    // ================================
    // References
    // ================================
    [Header("References")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    // ================================
    // Public API
    // ================================
    public bool[,,] SolidGrid { get; private set; }
    public float VoxelSize => voxelSize;
    public int YOffset => yOffset;
    public Transform ContentRoot => contentRoot;
    public bool IsBuildCompleted { get; private set; }

    // ================================
    // Internal
    // ================================
    private int[,,] csvGrid;

    // ================================
    // Build (Runtime / Editor 共通)
    // ================================
    public void BuildRoom()
    {
        IsBuildCompleted = false;

        if (!ValidateReferences())
            return;

        // ContentRoot は必ずローカル原点
        contentRoot.localPosition = Vector3.zero;
        contentRoot.localRotation = Quaternion.identity;
        contentRoot.localScale = Vector3.one;

        ClearContent();
        LoadCsv3D();

        if (csvGrid == null)
        {
            Debug.LogError("[RoomBuilder] csvGrid is NULL", this);
            return;
        }

        int width = csvGrid.GetLength(0);
        int height = csvGrid.GetLength(1);
        int depth = csvGrid.GetLength(2);

        SolidGrid = new bool[width, height, depth];

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    int csv = csvGrid[x, y, z];
                    if (csv == 0)
                        continue;

                    Vector3 localPos = new Vector3(
                        (x + 0.5f) * voxelSize,
                        (y + yOffset + 0.5f) * voxelSize,
                        (z + 0.5f) * voxelSize
                    );

                    switch (csv)
                    {
                        case 1: // 床
                        case 2: // 壁
                            {
                                GameObject prefab =
                                    BlockFactory.GetPrefab((char)('0' + csv));
                                if (prefab == null)
                                    break;

                                var obj = Instantiate(prefab, contentRoot);
                                obj.transform.localPosition = localPos;
                                obj.transform.localRotation = Quaternion.identity;

                                SolidGrid[x, y, z] = true;
                                break;
                            }

                        case 4: // 穴底（見た目のみ）
                            {
                                GameObject prefab = BlockFactory.GetPrefab('4');
                                if (prefab == null)
                                    break;

                                var obj = Instantiate(prefab, contentRoot);
                                obj.transform.localPosition = localPos;
                                obj.transform.localRotation = Quaternion.identity;
                                break;
                            }

                        case 3:
                            // ★ carryblock は生成しない
                            // ResettableStageController が管理する
                            break;
                    }
                }
            }
        }

        RebuildColliders();
        IsBuildCompleted = true;
    }

    // ================================
    // Runtime 用 API
    // ================================
    public TerrainState BuildTerrain()
    {
        BuildRoom();
        return new TerrainState(csvGrid, SolidGrid, voxelSize, yOffset);
    }

#if UNITY_EDITOR
    // ================================
    // Editor 専用 API
    // ================================
    /// <summary>
    /// Editor 専用：Metadata を差し替えて部屋を構築する
    /// Runtime からは呼ばない
    /// </summary>
    public void BuildForEditor(RoomMetadata metadata)
    {
        if (metadataHolder == null)
        {
            metadataHolder = GetComponentInChildren<RoomMetadataHolder>();
            if (metadataHolder == null)
            {
                Debug.LogError("[RoomBuilder] RoomMetadataHolder not found", this);
                return;
            }
        }

        metadataHolder.metadata = metadata;
        BuildRoom();
    }
#endif

    // ================================
    // CSV Loader
    // ================================
    private void LoadCsv3D()
    {
        TextAsset csv = metadataHolder.metadata.roomCsv;
        string[] rawLines = csv.text.Replace("\r", "").Split('\n');

        var layers = new List<List<string>>();
        var current = new List<string>();

        foreach (var line in rawLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("---"))
            {
                if (current.Count > 0)
                {
                    layers.Add(current);
                    current = new List<string>();
                }
                continue;
            }

            current.Add(line);
        }

        if (current.Count > 0)
            layers.Add(current);

        int height = layers.Count;
        int depth = layers[0].Count;
        int width = layers[0][0].Length;

        csvGrid = new int[width, height, depth];

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                string line = layers[y][z];
                for (int x = 0; x < width; x++)
                {
                    csvGrid[x, y, depth - 1 - z] = line[x] - '0';
                }
            }
        }
    }

    // ================================
    // Collider
    // ================================
    private void RebuildColliders()
    {
        VoxelColliderUtility.BuildColliders(
            contentRoot,
            SolidGrid,
            voxelSize,
            yOffset,
            this
        );
    }

    // ================================
    // Utility
    // ================================
    private void ClearContent()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(contentRoot.GetChild(i).gameObject);
            else
                Destroy(contentRoot.GetChild(i).gameObject);
#else
            Destroy(contentRoot.GetChild(i).gameObject);
#endif
        }
    }

    private bool ValidateReferences()
    {
        if (contentRoot == null)
            return false;

        if (metadataHolder == null)
            metadataHolder = GetComponentInChildren<RoomMetadataHolder>();

        return metadataHolder != null &&
               metadataHolder.metadata != null;
    }
}
