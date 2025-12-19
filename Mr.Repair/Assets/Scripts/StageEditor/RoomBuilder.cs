using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    [Header("References")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    public bool[,,] SolidGrid { get; private set; }
    public float VoxelSize => voxelSize;
    public int YOffset => yOffset;
    public Transform ContentRoot => contentRoot;

    private bool colliderDirty;

    // CSV 3D
    private int[,,] csvGrid;

    // ================================
    // Build
    // ================================
    public void BuildRoom()
    {
        if (!ValidateReferences())
            return;

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
                    if (csv == 0) continue;

                    // ★ collider と完全一致する localPosition（セル中心）
                    Vector3 localPos = new Vector3(
                        (x + 0.5f) * voxelSize,
                        (y + yOffset + 0.5f) * voxelSize,
                        (z + 0.5f) * voxelSize
                    );

                    GameObject prefab =
                        BlockFactory.GetPrefab((char)('0' + csv));
                    if (prefab == null) continue;

                    // ★ 親のみ指定 → localPosition で揃える
                    var obj = Instantiate(prefab, contentRoot);
                    obj.transform.localPosition = localPos;
                    obj.transform.localRotation = Quaternion.identity;

                    switch (csv)
                    {
                        case 1: // ground
                        case 2: // goal
                        case 4: // hole bottom（地面扱い）
                            SolidGrid[x, y, z] = true;

                            if (csv == 4)
                                obj.tag = "Ground";
                            break;

                        case 3: // carry block
                            var pushable = obj.GetComponent<PushableBlock>();
                            if (pushable != null)
                                pushable.SetOwner(this);
                            break;
                    }
                }
            }
        }

        RebuildColliders();
    }

    // ================================
    // CSV 3D 読み込み
    // ================================
    private void LoadCsv3D()
    {
        TextAsset csv = metadataHolder.metadata.roomCsv;

        string[] rawLines = csv.text
            .Replace("\r", "")
            .Split('\n');

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

        if (layers.Count == 0)
        {
            Debug.LogError("[RoomBuilder] CSV has no layers", this);
            return;
        }

        int height = layers.Count;
        int depth = layers[0].Count;
        int width = layers[0][0].Length;

        csvGrid = new int[width, height, depth];

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                string line = layers[y][z];

                if (line.Length != width)
                {
                    Debug.LogError(
                        $"[RoomBuilder] CSV width mismatch at layer {y}, row {z}",
                        this
                    );
                    return;
                }

                for (int x = 0; x < width; x++)
                {
                    csvGrid[x, y, depth - 1 - z] = line[x] - '0';
                }
            }
        }

        Debug.Log(
            $"[RoomBuilder] CSV Loaded {width} x {height} x {depth}",
            this
        );
    }

    // ================================
    // Hole fill
    // ================================
    public void FillHole(int x, int y, int z)
    {
        if (SolidGrid[x, y, z])
            return;

        SolidGrid[x, y, z] = true;

        Vector3 localPos = new Vector3(
            (x + 0.5f) * voxelSize,
            (y + yOffset + 0.5f) * voxelSize,
            (z + 0.5f) * voxelSize
        );

        GameObject groundPrefab = BlockFactory.GetPrefab('1');
        if (groundPrefab != null)
        {
            var obj = Instantiate(groundPrefab, contentRoot);
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = Quaternion.identity;
        }

        colliderDirty = true;
    }

    private void LateUpdate()
    {
        if (!colliderDirty) return;
        colliderDirty = false;
        RebuildColliders();
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

        EnsureRoomColliderOwner();
    }

    private void EnsureRoomColliderOwner()
    {
        var owner = contentRoot.GetComponent<RoomColliderOwner>();
        if (owner == null)
            owner = contentRoot.gameObject.AddComponent<RoomColliderOwner>();

        owner.Owner = this;
    }

    private void ClearContent()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(contentRoot.GetChild(i).gameObject);
    }

    private bool ValidateReferences()
    {
        if (contentRoot == null)
        {
            Debug.LogError("[RoomBuilder] contentRoot is NULL", this);
            return false;
        }

        if (metadataHolder == null)
            metadataHolder = GetComponentInChildren<RoomMetadataHolder>();

        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogError("[RoomBuilder] metadata missing", this);
            return false;
        }

        return true;
    }
}
