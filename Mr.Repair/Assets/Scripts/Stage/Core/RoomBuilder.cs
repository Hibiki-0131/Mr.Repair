using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomBuilder : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    [Header("References")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    public bool[,,] SolidGrid { get; private set; }
    public Transform ContentRoot => contentRoot;
    public float VoxelSize => voxelSize;
    public int YOffset => yOffset;

    private int[,,] csvGrid;

    public void SetContentRoot(Transform root)
    {
        contentRoot = root;
    }

    // ================================
    // Build (Editor / Runtime 共通)
    // ================================
    public void BuildRoom()
    {
        if (!ValidateReferences())
            return;

        ClearContent();
        LoadCsv3D();

        int w = csvGrid.GetLength(0);
        int h = csvGrid.GetLength(1);
        int d = csvGrid.GetLength(2);

        SolidGrid = new bool[w, h, d];

        for (int y = 0; y < h; y++)
            for (int z = 0; z < d; z++)
                for (int x = 0; x < w; x++)
                {
                    int csv = csvGrid[x, y, z];

                    switch (csv)
                    {
                        case 1: // 通常床
                            Instantiate(
                                BlockFactory.GetPrefab('1'),
                                contentRoot
                            ).transform.localPosition = GridToLocal(x, y, z);

                            SolidGrid[x, y, z] = true;
                            break;

                        case 2: // ゴール床
                            Instantiate(
                                BlockFactory.GetPrefab('2'),
                                contentRoot
                            ).transform.localPosition = GridToLocal(x, y, z);

                            SolidGrid[x, y, z] = true;
                            break;

                        default:
                            // 0,3 は床なし
                            SolidGrid[x, y, z] = false;
                            break;
                    }
                }

        RebuildColliders();
    }

    public TerrainState BuildTerrain()
    {
        BuildRoom();
        return new TerrainState(csvGrid, SolidGrid, voxelSize, yOffset);
    }

    // ================================
    // CarryBlock 初期生成（★追加）
    // ================================
    public void SpawnInitialCarryBlocks(
        GameObject carryBlockPrefab,
        SettlementCoordinator settlementCoordinator
    )
    {
        if (carryBlockPrefab == null)
        {
            Debug.LogError("[RoomBuilder] CarryBlock prefab is null", this);
            return;
        }

        if (settlementCoordinator == null)
        {
            Debug.LogError("[RoomBuilder] SettlementCoordinator is null", this);
            return;
        }

        foreach (Vector3 pos in GetCarryBlockPositions())
        {
            var block = Instantiate(
                carryBlockPrefab,
                pos,
                Quaternion.identity,
                contentRoot
            );

            var sensor = block.GetComponent<BlockSettlementSensor>();
            if (sensor != null)
            {
                sensor.SetCoordinator(settlementCoordinator);
            }
            else
            {
                Debug.LogError(
                    "[RoomBuilder] BlockSettlementSensor missing on CarryBlock",
                    block
                );
            }
        }
    }

    // ================================
    // CSV 解釈（CarryBlock 用）
    // ================================
    public IEnumerable<Vector3> GetCarryBlockPositions()
    {
        int w = csvGrid.GetLength(0);
        int h = csvGrid.GetLength(1);
        int d = csvGrid.GetLength(2);

        for (int y = 0; y < h; y++)
            for (int z = 0; z < d; z++)
                for (int x = 0; x < w; x++)
                    if (csvGrid[x, y, z] == 3)
                        yield return GridToLocal(x, y, z);
    }

#if UNITY_EDITOR
    public void BuildForEditor(RoomMetadata metadata)
    {
        if (metadataHolder == null)
            metadataHolder = GetComponentInChildren<RoomMetadataHolder>();

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
                layers.Add(current);
                current = new List<string>();
                continue;
            }

            current.Add(line);
        }
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

        return metadataHolder != null && metadataHolder.metadata != null;
    }
}
