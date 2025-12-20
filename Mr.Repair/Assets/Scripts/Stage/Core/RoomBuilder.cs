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
    public Transform ContentRoot => contentRoot;
    public float VoxelSize => voxelSize;
    public int YOffset => yOffset;

    // ================================
    // Internal
    // ================================
    private int[,,] csvGrid;

    // ================================
    // ContentRoot 注入（Editor 用）
    // ================================
    public void SetContentRoot(Transform root)
    {
        contentRoot = root;
    }

    // ================================
    // Build（Runtime / Editor 共通）
    // ================================
    public void BuildRoom()
    {
        if (!ValidateReferences())
            return;

        contentRoot.localPosition = Vector3.zero;
        contentRoot.localRotation = Quaternion.identity;
        contentRoot.localScale = Vector3.one;

        ClearContent();
        LoadCsv3D();

        int w = csvGrid.GetLength(0);
        int h = csvGrid.GetLength(1);
        int d = csvGrid.GetLength(2);

        SolidGrid = new bool[w, h, d];

        for (int y = 0; y < h; y++)
        {
            for (int z = 0; z < d; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    int csv = csvGrid[x, y, z];
                    if (csv == 0)
                        continue;

                    Vector3 pos = GridToLocal(x, y, z);

                    switch (csv)
                    {
                        case 1: // floor
                        case 2: // wall
                            Instantiate(
                                BlockFactory.GetPrefab((char)('0' + csv)),
                                contentRoot
                            ).transform.localPosition = pos;

                            SolidGrid[x, y, z] = true;
                            break;

                        case 4: // hole bottom (mesh only)
                            Instantiate(
                                BlockFactory.GetPrefab('4'),
                                contentRoot
                            ).transform.localPosition = pos;
                            break;

                        case 3:
                            // ★ carryblock はここでは生成しない
                            break;
                    }
                }
            }
        }

        RebuildColliders();
    }

    // ================================
    // Runtime API
    // ================================
    public TerrainState BuildTerrain()
    {
        BuildRoom();
        return new TerrainState(csvGrid, SolidGrid, voxelSize, yOffset);
    }

    // ================================
    // ★ 追加：carryblock 初期位置提供 API
    // ================================
    public IEnumerable<Vector3> GetCarryBlockPositions()
    {
        if (csvGrid == null)
            yield break;

        int w = csvGrid.GetLength(0);
        int h = csvGrid.GetLength(1);
        int d = csvGrid.GetLength(2);

        for (int y = 0; y < h; y++)
        {
            for (int z = 0; z < d; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (csvGrid[x, y, z] == 3)
                    {
                        yield return GridToLocal(x, y, z);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    // ================================
    // Editor 専用 API
    // ================================
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
    // Internal
    // ================================
    private Vector3 GridToLocal(int x, int y, int z)
    {
        return new Vector3(
            (x + 0.5f) * voxelSize,
            (y + yOffset + 0.5f) * voxelSize,
            (z + 0.5f) * voxelSize
        );
    }

    private void LoadCsv3D()
    {
        TextAsset csv = metadataHolder.metadata.roomCsv;
        string[] lines = csv.text.Replace("\r", "").Split('\n');

        var layers = new List<List<string>>();
        var current = new List<string>();

        foreach (var line in lines)
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

        int h = layers.Count;
        int d = layers[0].Count;
        int w = layers[0][0].Length;

        csvGrid = new int[w, h, d];

        for (int y = 0; y < h; y++)
            for (int z = 0; z < d; z++)
                for (int x = 0; x < w; x++)
                    csvGrid[x, y, d - 1 - z] = layers[y][z][x] - '0';
    }

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
