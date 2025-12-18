using UnityEngine;
using System.Collections;

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

    // ★ 自動 Build はしない（StageEditor / Reset からのみ呼ぶ）
    private void Start(){
        BuildRoom();
    }

    /// <summary>
    /// CSV から Room を再構築する
    /// （見た目・論理・Collider を local 座標で完全同期）
    /// </summary>
    public void BuildRoom()
    {
        if (metadataHolder == null || metadataHolder.metadata == null)
        {
            Debug.LogWarning("RoomMetadata が設定されていません");
            return;
        }

        // 既存の地形を削除（CarryBlock は残す）
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
        string[] layers = csv.Split(
            new string[] { "---" },
            System.StringSplitOptions.RemoveEmptyEntries);

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

                    Vector3 localPos =
                        new Vector3(x, y + yOffset, zr) * voxelSize;

                    // CarryBlock（Collider 統合対象外）
                    if (code == '3')
                    {
                        if (prefab != null)
                        {
                            var block = Instantiate(prefab, contentRoot);
                            block.transform.localPosition = localPos;
                            block.transform.localRotation = Quaternion.identity;
                        }

                        solid[x, y, zr] = false;
                        continue;
                    }

                    // Static Block
                    if (prefab != null)
                    {
                        var block = Instantiate(prefab, contentRoot);
                        block.transform.localPosition = localPos;
                        block.transform.localRotation = Quaternion.identity;

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

        // Collider を solid 配列から再生成（local）
        VoxelColliderUtility.BuildColliders(
            contentRoot, solid, voxelSize, yOffset);

        Debug.Log("Room Build Complete");
    }

    /// <summary>
    /// PushableBlock が穴に落ちたときに呼ばれる
    /// </summary>
    public void FillHole(int x, int y, int z)
    {
        if (solid == null)
            return;

        solid[x, y, z] = true;

        // Collider 再構築は安全なタイミングで
        StartCoroutine(RebuildLater());
    }

    private IEnumerator RebuildLater()
    {
        yield return new WaitForEndOfFrame();

        VoxelColliderUtility.BuildColliders(
            contentRoot, solid, voxelSize, yOffset);

        Debug.Log("Collider Re-Built");
    }
}
