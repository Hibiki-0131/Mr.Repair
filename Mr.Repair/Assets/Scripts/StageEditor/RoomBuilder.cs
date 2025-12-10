using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public static RoomBuilder Instance { get; private set; }

    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

    private bool[,,] solid;

    public bool[,,] SolidGrid => solid;
    public float VoxelSize => voxelSize;
    public int YOffset => yOffset;
    public Transform ContentRoot => contentRoot;

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

        // ★ 地形だけ破棄、PushableBlock は残す
        foreach (Transform child in contentRoot)
        {
            if (child.GetComponent<PushableBlock>() != null)
                continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        string csv = metadataHolder.metadata.roomCsv.text.Replace("\r", "");
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        string[] firstLines = layers[0].Trim().Split('\n');
        int depth = firstLines.Length;
        int width = firstLines[0].Trim().Length;
        int height = layers.Length;

        solid = new bool[width, height, depth];

        int y = 0;
        foreach (var layer in layers)
        {
            string[] lines = layer.Trim().Split('\n');

            for (int z = 0; z < lines.Length; z++)
            {
                string line = lines[z].Trim();
                int zr = (lines.Length - 1) - z;

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    // CSV '3': CarryBlock → Instantiate するが static collider に含めない
                    if (code == '3')
                    {
                        if (prefab != null)
                        {
                            Vector3 pos = new Vector3(x, y + yOffset, zr) * voxelSize;
                            Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                        }
                        solid[x, y, zr] = false;
                        continue;
                    }

                    // Static Blocks
                    if (prefab != null)
                    {
                        Vector3 pos = new Vector3(x, y + yOffset, zr) * voxelSize;
                        Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                        solid[x, y, zr] = true;
                    }
                    else
                    {
                        solid[x, y, zr] = false;
                    }
                }
            }
            y++;
        }

        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
        Debug.Log("Room Build Complete");
    }

    public void FillHole(int x, int y, int z)
    {
        solid[x, y, z] = true;

        StartCoroutine(RebuildLater());
    }

    private System.Collections.IEnumerator RebuildLater()
    {
        yield return new WaitForEndOfFrame();
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
        Debug.Log("Collider Re-Built");
    }
}
