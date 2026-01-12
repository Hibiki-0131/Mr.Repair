using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BlockFactoryInitializer : MonoBehaviour
{
    public GameObject floorPrefab;      // csv=1
    public GameObject goalPrefab;       // csv=2
    public GameObject carryBlockPrefab; // csv=3

    private void Awake() => Init();
    private void OnEnable() => Init();

    private void Init()
    {
        // プレハブの割り当てチェック
        if (floorPrefab == null || goalPrefab == null || carryBlockPrefab == null)
        {
            // ビルド時（エディタでない時）または再生中にのみエラーを出す
            // これにより、編集中のインスペクター未設定によるビルド失敗を防ぎやすくなります
            if (Application.isPlaying)
            {
                Debug.LogError(
                    $"[BlockFactoryInitializer] 割り当てられていないプレハブがあります: " +
                    $"Floor: {floorPrefab}, Goal: {goalPrefab}, Carry: {carryBlockPrefab}",
                    this
                );
            }
            return;
        }

        // 全て揃っている場合のみ辞書を初期化
        BlockFactory.Initialize(new Dictionary<char, GameObject>()
        {
            { '1', floorPrefab },
            { '2', goalPrefab },
            { '3', carryBlockPrefab },
            { '0', null },
        });
    }
}