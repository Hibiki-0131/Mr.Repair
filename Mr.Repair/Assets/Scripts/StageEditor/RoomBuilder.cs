using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    // CSV 1マスのサイズ
    [SerializeField] private float voxelSize = 1f;
    // y方向にオフセットしたいならここを 1 にするなど
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

        // 既存の子オブジェクト削除
        foreach (Transform child in contentRoot)
            DestroyImmediate(child.gameObject);

        string csv = metadataHolder.metadata.roomCsv.text;
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (layers.Length == 0)
        {
            Debug.LogWarning("CSV にレイヤーがありません");
            return;
        }

        // 1層目から幅・奥行きを決める
        string[] firstLines = layers[0].Trim().Split('\n');
        int depth = firstLines.Length;              // Z方向（CSVの行）
        int width = firstLines[0].Trim().Length;   // X方向（CSVの列）
        int height = layers.Length;                // Y方向（層）

        // solid[x,y,z] = その位置にブロックがあるか
        bool[,,] solid = new bool[width, height, depth];

        int currentY = 0;
        foreach (var layer in layers)
        {
            string[] lines = layer.Trim().Split('\n');

            Debug.Log($"Building layer {currentY}: first line = {lines[0]}");

            for (int z = 0; z < lines.Length; z++)
            {
                string line = lines[z].Trim();

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    if (prefab != null)
                    {
                        // ブロックを生成（見た目）
                        Vector3 pos = new Vector3(x, currentY + yOffset, z) * voxelSize;
                        Instantiate(prefab, pos, Quaternion.identity, contentRoot);

                        // グリッドに固体として記録
                        solid[x, currentY, z] = true;
                    }
                }
            }

            currentY++;
        }

        // コライダーを voxel 最適化して生成
        VoxelColliderUtility.BuildColliders(contentRoot, solid, voxelSize, yOffset);

        Debug.Log("Room build complete.");
    }
}
