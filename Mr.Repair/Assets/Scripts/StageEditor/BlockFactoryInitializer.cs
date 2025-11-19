using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]   // ← これを追加
public class BlockFactoryInitializer : MonoBehaviour
{
    [Header("Block Prefab Settings")]
    public GameObject wallPrefab;   // CSV '1' 用（Tile_Floor でOK, 1×1×1なら）
    public GameObject floorPrefab;  // 使わないなら null のままでOK

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if (wallPrefab == null)
        {
            Debug.LogWarning("BlockFactoryInitializer: wallPrefab が設定されていません");
            return;
        }

        var map = new Dictionary<char, GameObject>()
        {
            { '1', wallPrefab },  // ブロックを置く
            { '0', null },        // 何も置かない → 穴
        };

        BlockFactory.Initialize(map);
        Debug.Log("BlockFactory initialized.");
    }
}
