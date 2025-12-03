using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    private bool[,,] solid;  // ← 穴埋めに使うグリッドデータ

    // 他クラス（PushableBlock）から参照するための公開プロパティ
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
        BuildRoom();  // ← 追加！
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

    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogWarning("RoomMetadata が設定されていません");
            return;
        }

        // Child の削除
        foreach (Transform child in contentRoot)
            DestroyImmediate(child.gameObject);

        string csv = metadataHolder.metadata.roomCsv.text;

        // ★ 改行コード CR を削除してから split する
        csv = csv.Replace("\r", "");

        // レイヤーで区切る
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (layers.Length == 0)
        {
            Debug.LogWarning("CSV にレイヤーがありません");
            return;
        }

        // 1層目から幅と奥行きを算出
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

                        Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                        solid[x, currentY, zReversed] = true;
                    }
                }
            }

            currentY++;
        }

        // voxel 最適化コリジョン生成
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
        Debug.Log("Room build complete.");
    }

    // ★ 穴を埋める（carryblock が落ちたとき呼ぶ）
    public void FillHole(int x, int y, int z)
    {
        if (solid == null) return;

        solid[x, y, z] = true;

        // コライダー再構築
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);

        Debug.Log($"Hole filled at {x},{y},{z}");
    }
}
