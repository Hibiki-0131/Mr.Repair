using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [Header("Voxel Settings")]
    public float voxelSize = 1f;
    public int yOffset = 0;

    // 外部から block 配置を参照できる
    public bool[,,] solid;
    public int width;
    public int heightCount;
    public int depth;

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

        foreach (Transform child in contentRoot)
            DestroyImmediate(child.gameObject);

        string csv = metadataHolder.metadata.roomCsv.text.Replace("\r", "");
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (layers.Length == 0)
        {
            Debug.LogWarning("CSV にレイヤーがありません");
            return;
        }

        string[] firstLines = layers[0].Trim().Split('\n');
        depth = firstLines.Length;
        width = firstLines[0].Trim().Length;
        heightCount = layers.Length;

        solid = new bool[width, heightCount, depth];

        int currentY = 0;

        foreach (var layer in layers)
        {
            string[] lines = layer.Replace("\r", "").Trim().Split('\n');

            for (int z = 0; z < lines.Length; z++)
            {
                string line = lines[z].Trim();
                int zReversed = (lines.Length - 1) - z;

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    if (prefab != null)
                    {
                        Vector3 pos = new Vector3(x, currentY + yOffset, zReversed) * voxelSize;
                        var obj = Instantiate(prefab, pos, Quaternion.identity, contentRoot);

                        // CarryBlock 初期化
                        var pb = obj.GetComponent<PushableBlock>();
                        if (pb != null)
                            pb.Init(this);

                        solid[x, currentY, zReversed] = true;
                    }
                }
            }

            currentY++;
        }

        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
        Debug.Log("Room build complete.");
    }

    // 穴埋め用
    public void SetSolidVoxel(int x, int y, int z, bool value)
    {
        if (x < 0 || x >= width) return;
        if (y < 0 || y >= heightCount) return;
        if (z < 0 || z >= depth) return;

        solid[x, y, z] = value;
    }

    // コライダー再生成
    public void RebuildColliders()
    {
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
    }
}
