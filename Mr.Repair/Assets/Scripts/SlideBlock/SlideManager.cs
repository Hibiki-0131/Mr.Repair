using UnityEngine;
using System.Collections.Generic;

public class SlideManager : MonoBehaviour
{
    public static SlideManager Instance { get; private set; }

    private Dictionary<string, SlideBlock> blockDictionary = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // シーン内の全SlideBlockを登録
        foreach (var block in FindObjectsOfType<SlideBlock>())
        {
            if (!string.IsNullOrEmpty(block.BlockID))
            {
                blockDictionary[block.BlockID] = block;
            }
        }
    }

    public void ActivateBlock(string id)
    {
        if (blockDictionary.TryGetValue(id, out SlideBlock block))
        {
            block.Activate();
        }
        else
        {
            Debug.LogWarning($"ID:{id} に対応するブロックが見つかりません。");
        }
    }
}
