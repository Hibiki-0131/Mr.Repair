using System.Collections.Generic;
using UnityEngine;

public class BlockFactoryInitializer : MonoBehaviour
{
    [Header("Block Prefab Settings")]
    public GameObject wallPrefab;   // CSVの '1' 用
    public GameObject floorPrefab;  // 必要なら '2' など追加
    // public GameObject trapPrefab; // 追加の種類もここに

    private void Awake()
    {
        var map = new Dictionary<char, GameObject>()
        {
            { '1', wallPrefab },  // 壁
            { '0', null },        // 0は空白（生成しない）
            // { '2', trapPrefab },
        };

        BlockFactory.Initialize(map);
    }
}
