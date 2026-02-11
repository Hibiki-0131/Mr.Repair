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
    private int[,,] colorGrid;

    // =========================
    // ★ 安全キャッシュ
    // =========================
    private void Awake()
    {
        ResolveMetadataHolder();
    }

    private void ResolveMetadataHolder()
    {
        if (metadataHolder == null)
            metadataHolder = GetComponent<RoomMetadataHolder>();

        if (metadataHolder == null)
            Debug.LogError("[RoomBuilder] RoomMetadataHolder NOT FOUND", this);
    }

    public void SetContentRoot(Transform root) => contentRoot = root;

    public void BuildRoom()
    {
        if (!ValidateReferences()) return;

        ClearContent();
        LoadAllCsvs();

        int w = csvGrid.GetLength(0);
        int h = csvGrid.GetLength(1);
        int d = csvGrid.GetLength(2);
        SolidGrid = new bool[w, h, d];

        for (int y = 0; y < h; y++)
            for (int z = 0; z < d; z++)
                for (int x = 0; x < w; x++)
                {
                    int csv = csvGrid[x, y, z];
                    int colorIdx = colorGrid[x, y, z];

                    if (csv == 1 || csv == 2)
                    {
                        var prefab = BlockFactory.GetPrefab(csv == 1 ? '1' : '2');
                        var go = Instantiate(prefab, contentRoot);
                        go.transform.localPosition = GridToLocal(x, y, z);

                        ApplyColorByIndex(go, colorIdx);
                        SolidGrid[x, y, z] = true;
                    }
                    else
                    {
                        SolidGrid[x, y, z] = false;
                    }
                }

        RebuildColliders();
    }

    // =========================
    // Color Apply
    // =========================
    public void ApplyColorByIndex(GameObject target, int colorIdx)
    {
        if (metadataHolder?.metadata == null) return;

        var meta = metadataHolder.metadata;

        Color targetColor = meta.defaultFloorColor;
        if (colorIdx > 0 && colorIdx <= meta.floorPalette.Length)
        {
            targetColor = meta.floorPalette[colorIdx - 1];
        }

        var renderer = target.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_Color", targetColor);
            renderer.SetPropertyBlock(prop);
        }
    }

    public int GetColorIndexAt(Vector3 localPos)
    {
        int x = Mathf.FloorToInt(localPos.x / voxelSize);
        int y = Mathf.FloorToInt(localPos.y / voxelSize) - yOffset;
        int z = Mathf.FloorToInt(localPos.z / voxelSize);

        if (colorGrid != null &&
            x >= 0 && x < colorGrid.GetLength(0) &&
            y >= 0 && y < colorGrid.GetLength(1) &&
            z >= 0 && z < colorGrid.GetLength(2))
        {
            return colorGrid[x, y, z];
        }

        return 0;
    }

    private void LoadAllCsvs()
    {
        var meta = metadataHolder.metadata;

        csvGrid = ParseCsv(meta.roomCsv);

        if (meta.colorCsv != null)
            colorGrid = ParseCsv(meta.colorCsv);
        else
            colorGrid = new int[csvGrid.GetLength(0), csvGrid.GetLength(1), csvGrid.GetLength(2)];
    }

    private int[,,] ParseCsv(TextAsset csvAsset)
    {
        string[] lines = csvAsset.text.Replace("\r", "").Split('\n');

        var layers = new List<List<string>>();
        var current = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

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

        int[,,] grid = new int[w, h, d];

        for (int y = 0; y < h; y++)
            for (int z = 0; z < d; z++)
                for (int x = 0; x < w; x++)
                    grid[x, y, d - 1 - z] = layers[y][z][x] - '0';

        return grid;
    }

    public TerrainState BuildTerrain()
    {
        BuildRoom();
        return new TerrainState(csvGrid, SolidGrid, voxelSize, yOffset);
    }

    public void SpawnInitialCarryBlocks(GameObject prefab, SettlementCoordinator sc)
    {
        foreach (var localPos in GetCarryBlockPositions())
        {
            Vector3 worldPos = contentRoot.TransformPoint(localPos);

            var b = Instantiate(prefab, worldPos, Quaternion.identity, contentRoot);

            b.GetComponent<BlockSettlementSensor>()?.SetCoordinator(sc);
        }
    }

    public IEnumerable<Vector3> GetCarryBlockPositions()
    {
        for (int y = 0; y < csvGrid.GetLength(1); y++)
            for (int z = 0; z < csvGrid.GetLength(2); z++)
                for (int x = 0; x < csvGrid.GetLength(0); x++)
                    if (csvGrid[x, y, z] == 3)
                        yield return GridToLocal(x, y, z);
    }

#if UNITY_EDITOR
    public void BuildForEditor(RoomMetadata m)
    {
        ResolveMetadataHolder();

        if (metadataHolder != null)
            metadataHolder.metadata = m;

        BuildRoom();
    }
#endif

    private Vector3 GridToLocal(int x, int y, int z)
    {
        return new Vector3(
            (x + 0.5f) * voxelSize,
            (y + yOffset + 0.5f) * voxelSize,
            (z + 0.5f) * voxelSize
        );
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
            if (!Application.isPlaying)
                DestroyImmediate(contentRoot.GetChild(i).gameObject);
            else
                Destroy(contentRoot.GetChild(i).gameObject);
        }
    }

    private bool ValidateReferences()
    {
        if (contentRoot == null)
        {
            Debug.LogError("[RoomBuilder] ContentRoot is NULL", this);
            return false;
        }

        ResolveMetadataHolder();

        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogError("[RoomBuilder] Metadata missing", this);
            return false;
        }

        return true;
    }
}
