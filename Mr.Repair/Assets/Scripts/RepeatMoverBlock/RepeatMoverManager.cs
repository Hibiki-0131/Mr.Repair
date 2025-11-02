using UnityEngine;
using System.Collections.Generic;

public class RepeatMoverManager : MonoBehaviour
{
    public static RepeatMoverManager Instance { get; private set; }

    private Dictionary<string, List<RepeatMoverBlock>> blockDictionary = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // シーン内の全RepeatMoverBlockを登録
        foreach (var block in FindObjectsOfType<RepeatMoverBlock>())
        {
            if (!string.IsNullOrEmpty(block.BlockID))
            {
                if (!blockDictionary.ContainsKey(block.BlockID))
                    blockDictionary[block.BlockID] = new List<RepeatMoverBlock>();

                blockDictionary[block.BlockID].Add(block);
            }
        }
    }

    public void ToggleBlock(string id)
    {
        if (blockDictionary.TryGetValue(id, out List<RepeatMoverBlock> blocks))
        {
            foreach (var block in blocks)
                block.ToggleRepeat();
        }
        else
        {
            Debug.LogWarning($"ID:{id} に対応するブロックが見つかりません。");
        }
    }
}
