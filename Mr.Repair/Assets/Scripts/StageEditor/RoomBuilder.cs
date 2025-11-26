using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private int yOffset = 0;

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

        string csv = metadataHolder.metadata.roomCsv.text;

        // ★ CRLF を除去してから Split するのが重要！
        csv = csv.Replace("\r", "");

        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (layers.Length == 0)
        {
            Debug.LogWarning("CSV にレイヤーがありません");
            return;
        }

        string[] firstLines = layers[0].Trim().Split('\n');
        int depth = firstLines.Length;
        int width = firstLines[0].Trim().Length;
        int height = layers.Length;

        bool[,,] solid = new bool[width, height, depth];

        int currentY = 0;
        foreach (var layer in layers)
        {
            // ★ layer ごとにも CR 削除
            string[] lines = layer.Replace("\r", "").Trim().Split('\n');

            Debug.Log($"Building layer {currentY}, first line = '{lines[0]}'");

            for (int z = 0; z < lines.Length; z++)
            {
                string line = lines[z].Trim();
                Debug.Log($"line[{z}] = '{line}' length={line.Length}");

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    if (prefab != null)
                    {
                        Vector3 pos = new Vector3(x, currentY + yOffset, z) * voxelSize;
                        Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                        solid[x, currentY, z] = true;
                    }
                }
            }

            currentY++;
        }

        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);
        Debug.Log("Room build complete.");
    }
}
