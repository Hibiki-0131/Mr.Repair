using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    private bool[,,] solid;

    public bool[,,] SolidGrid => solid;
    public float VoxelSize => voxelSize;
    public int YOffset => yOffset;

    public static RoomBuilder Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BuildRoom();
    }

    private void Reset()
    {
        Transform root = transform.parent;
        if (root != null)
        {
            contentRoot = root.Find("ContentRoot");
            metadataHolder = root.GetComponent<RoomMetadataHolder>();
        }
    }

    // ============================================================
    //  ▼ ROOM BUILD (安定版)
    // ============================================================
    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogWarning("RoomMetadata が設定されていません");
            return;
        }

        // 古い地形だけ破棄（carryblockは残す）
        foreach (Transform child in contentRoot)
        {
            if (child.GetComponent<PushableBlock>() != null)
                continue;

            DestroyImmediate(child.gameObject);
        }

        string csv = metadataHolder.metadata.roomCsv.text.Replace("\r", "");
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (layers.Length == 0)
        {
            Debug.LogWarning("CSV にレイヤーがありません");
            return;
        }

        // マップサイズ
        string[] firstLines = layers[0].Trim().Split('\n');
        int depth = firstLines.Length;
        int width = firstLines[0].Trim().Length;
        int height = layers.Length;

        solid = new bool[width, height, depth];

        int currentY = 0;

        foreach (var layer in layers)
        {
            string[] lines = layer.Trim().Split('\n');

            for (int z = 0; z < lines.Length; z++)
            {
                string line = lines[z].Trim();

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    if (prefab == null)
                        continue;

                    // UI の z=0 が Unity の奥になるように反転
                    int zUnity = (lines.Length - 1) - z;

                    Vector3 pos = new Vector3(x, currentY + yOffset, zUnity) * voxelSize;

                    GameObject obj = Instantiate(prefab, pos, Quaternion.identity, contentRoot);

                    // ■ solid に登録（1 & 2 = 壁／床）
                    if (code == '1' || code == '2')
                    {
                        solid[x, currentY, zUnity] = true;
                    }

                    // ■ 3 = carry block
                    if (code == '3')
                    {
                        PushableBlock pb = obj.GetComponent<PushableBlock>();
                        if (pb != null)
                            pb.prefabReference = prefab;
                    }
                }
            }

            currentY++;
        }

        // voxel collider
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);

        Debug.Log("Room build complete.");
    }

    // ============================================================
    // ▼ Hole fill
    // ============================================================
    public void FillHole(int x, int y, int z)
    {
        if (solid == null)
            return;

        solid[x, y, z] = true;

        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);

        Debug.Log($"Hole filled at {x},{y},{z}");
    }
}
