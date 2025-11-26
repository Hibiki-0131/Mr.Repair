using UnityEngine;
using System.Collections.Generic;

public class SlideManager : MonoBehaviour
{
    public static SlideManager Instance { get; private set; }

    // 1つのIDに複数のSlideBlockを登録できるようにする
    private Dictionary<string, List<SlideBlock>> blockDictionary = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // シーン内の全SlideBlockを登録
        foreach (var block in FindObjectsOfType<SlideBlock>())
        {
            if (!string.IsNullOrEmpty(block.BlockID))
            {
                if (!blockDictionary.ContainsKey(block.BlockID))
                {
                    blockDictionary[block.BlockID] = new List<SlideBlock>();
                }
                blockDictionary[block.BlockID].Add(block);
            }
        }
    }

    public void ActivateBlock(string id)
    {
        if (blockDictionary.TryGetValue(id, out List<SlideBlock> blocks))
        {
            foreach (var block in blocks)
            {
                block.Activate();
            }
        }
        else
        {
            Debug.LogWarning($"ID:{id} に対応するブロックが見つかりません。");
        }
    }
}
