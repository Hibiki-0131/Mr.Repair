using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomMetadataHolder metadataHolder;

    private void Reset()
    {
        contentRoot = transform.Find("ContentRoot");
        metadataHolder = GetComponent<RoomMetadataHolder>();
    }

    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogWarning("RoomMetadata が設定されていません");
            return;
        }

        // 以前のブロックを消す
        foreach (Transform child in contentRoot)
            DestroyImmediate(child.gameObject);

        string csv = metadataHolder.metadata.roomCsv.text;
        string[] layers = csv.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

        int height = 0;
        foreach (var layer in layers)
        {
            string[] lines = layer.Trim().Split('\n');

            for (int y = 0; y < lines.Length; y++)
            {
                string line = lines[y].Trim();

                for (int x = 0; x < line.Length; x++)
                {
                    char code = line[x];
                    GameObject prefab = BlockFactory.GetPrefab(code);

                    if (prefab != null)
                    {
                        Vector3 pos = new Vector3(x, height, y);
                        Instantiate(prefab, pos, Quaternion.identity, contentRoot);
                    }
                }
            }
            height++;
        }
    }
}
