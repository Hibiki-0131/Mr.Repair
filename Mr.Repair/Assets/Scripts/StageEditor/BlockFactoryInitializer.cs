using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BlockFactoryInitializer : MonoBehaviour
{
    [Header("Block Prefab Settings")]
    public GameObject wallPrefab;      // CSV '1' 用
    public GameObject goalPrefab;      // CSV '2' 用
    public GameObject carryBlockPrefab; // CSV '3' 用（★追加）

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
        // 壁プレハブが未設定なら警告
        if (wallPrefab == null)
        {
            Debug.LogWarning("BlockFactoryInitializer: wallPrefab が設定されていません");
        }

        var map = new Dictionary<char, GameObject>()
        {
            { '1', wallPrefab },        // 壁
            { '2', goalPrefab },        // ゴール
            { '3', carryBlockPrefab },  // ★ CarryBlock（PushableBlock）
            { '0', null },              // 空白/穴
        };

        BlockFactory.Initialize(map);
        Debug.Log("BlockFactory initialized.");
    }
}
