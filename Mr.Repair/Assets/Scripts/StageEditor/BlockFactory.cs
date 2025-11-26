using System.Collections.Generic;
using UnityEngine;

public static class BlockFactory
{
    private static Dictionary<char, GameObject> blockMap;

    public static void Initialize(Dictionary<char, GameObject> map)
    {
        blockMap = map;
    }

    public static GameObject GetPrefab(char code)
    {
        if (blockMap == null)
        {
            Debug.LogWarning("BlockFactory ‚ª‰Šú‰»‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
            return null;
        }

        return blockMap.TryGetValue(code, out var prefab) ? prefab : null;
    }
}
