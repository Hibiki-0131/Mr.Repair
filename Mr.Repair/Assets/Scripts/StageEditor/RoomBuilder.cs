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

    public Transform ContentRoot => contentRoot;

    public static RoomBuilder Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BuildRoom();
    }

    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogWarning("RoomMetadata が設定されていません");
            return;
        }

        // 地形だけ消す（CarryBlockは残す）
        foreach (Transform child in contentRoot)
        {
            if (child.GetComponent<PushableBlock>() != null)
                continue;

            DestroyImmediate(child.gameObject);
        }

        string csv = metadataHolder.metadata.roomCsv.text.Replace("\r", "");
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

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
                    if (prefab == null) continue;

                    // ★ CSV '3' (CarryBlock) は生成しない
                    if (code == '3') continue;

                    int zReversed = (lines.Length - 1) - z;
                    Vector3 pos = new Vector3(x, currentY + yOffset, zReversed) * voxelSize;

                    Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                    solid[x, currentY, zReversed] = true;
                }
            }
            currentY++;
        }

        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
    }


    public void FillHole(int x, int y, int z)
    {
        solid[x, y, z] = true;
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
    }
}
