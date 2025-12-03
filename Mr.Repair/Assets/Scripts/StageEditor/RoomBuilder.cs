using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    private bool[,,] solid;  // ← 穴埋めや地形コライダー生成に使用

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
    //  ▼ ルーム構築
    // ============================================================
    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogWarning("RoomMetadata が設定されていません");
            return;
        }

        // ▼ 古いオブジェクト破棄
        foreach (Transform child in contentRoot)
        {
            DestroyImmediate(child.gameObject);
        }

        string csv = metadataHolder.metadata.roomCsv.text;
        csv = csv.Replace("\r", "");

        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);
        if (layers.Length == 0)
        {
            Debug.LogWarning("CSV にレイヤーがありません");
            return;
        }

        // ▼ マップサイズ
        string[] firstLines = layers[0].Trim().Split('\n');
        int depth = firstLines.Length;
        int width = firstLines[0].Trim().Length;
        int height = layers.Length;

        solid = new bool[width, height, depth];

        int currentY = 0;

        foreach (var layer in layers)
        {
            string[] lines = layer.Replace("\r", "").Trim().Split('\n');

            for (int z = 0; z < lines.Length; z++)
            {
                string line = lines[z].Trim();

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    if (prefab != null)
                    {
                        int zReversed = (lines.Length - 1) - z;
                        Vector3 pos = new Vector3(x, currentY + yOffset, zReversed) * voxelSize;

                        // 見た目 生成
                        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, contentRoot);

                        // ▼ '1' '2' のみ "地形" として solid に入れる
                        if (code == '1' || code == '2')
                        {
                            solid[x, currentY, zReversed] = true;
                        }

                        // ▼ PushableBlock の復元用に prefab を持たせる
                        PushableBlock pb = obj.GetComponent<PushableBlock>();
                        if (pb != null)
                        {
                            pb.prefabReference = prefab;
                        }
                    }
                }
            }

            currentY++;
        }

        // ▼ voxel 最適化コライダー生成
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);

        Debug.Log("Room build complete.");
    }

    // ============================================================
    //  ▼ 穴を埋める（carryblock が落ちた時に呼ぶ）
    // ============================================================
    public void FillHole(int x, int y, int z)
    {
        if (solid == null)
            return;

        solid[x, y, z] = true;

        // ▼ コライダー再構築
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);

        Debug.Log($"Hole filled at {x},{y},{z}");
    }
}
