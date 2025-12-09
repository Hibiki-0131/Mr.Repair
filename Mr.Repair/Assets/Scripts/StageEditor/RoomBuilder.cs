using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    private bool[,,] solid;
    public bool[,,] SolidGrid => solid;
    public Transform ContentRoot => contentRoot;

    public float VoxelSize => voxelSize;

    public static RoomBuilder Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null) return;

        // 地形のみ削除（CarryBlockは消さない）
        foreach (Transform child in contentRoot)
        {
            if (child.GetComponent<PushableBlock>() != null) continue;
            DestroyImmediate(child.gameObject);
        }

        string csv = metadataHolder.metadata.roomCsv.text.Replace("\r", "");
        string[] layers = csv.Split(new string[] { "---" },
                        System.StringSplitOptions.RemoveEmptyEntries);

        int height = layers.Length;
        int depth = layers[0].Trim().Split('\n').Length;
        int width = layers[0].Trim().Split('\n')[0].Trim().Length;

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

                    // 1,2 のみ地形
                    if (code == '1' || code == '2')
                    {
                        int zUnity = (lines.Length - 1) - z;
                        Vector3 pos = new Vector3(x, currentY + yOffset, zUnity) * voxelSize;
                        Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                        solid[x, currentY, zUnity] = true;
                    }
                }
            }
            currentY++;
        }

        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
        Debug.Log("Room build complete");
    }

    public void FillHole(int x, int y, int z)
    {
        if (solid == null) return;
        solid[x, y, z] = true;

        // コライダー再構築（最小限）
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
    }
}
