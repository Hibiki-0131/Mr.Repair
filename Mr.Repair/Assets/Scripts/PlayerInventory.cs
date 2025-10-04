using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<KeyItemData> allKeyItems;  // マップ上の全アイテム
    private List<KeyItemData> obtainedItems = new List<KeyItemData>();

    private void Start()
    {
        // UI にアイテムリストを渡して初期化
        KeyItemListUI.Instance.InitializeSlots(allKeyItems);
    }

    // アイテム取得
    public void ObtainItem(KeyItemData item)
    {
        obtainedItems.Add(item);  // ←重複チェックを削除
        Debug.Log($"{item.itemName} を取得しました。");

        // UI 更新
        KeyItemListUI.Instance.UpdateSlot(item, true);
    }

    // アイテム使用
    public void UseItem(KeyItemData item)
    {
        if (obtainedItems.Contains(item))
        {
            obtainedItems.Remove(item);  // 1つだけ削除
            Debug.Log($"{item.itemName} を使用しました。");

            // UI 更新（元の？アイコンに戻す）
            KeyItemListUI.Instance.UpdateSlot(item, false);
        }
    }

    // 取得済みか確認
    public bool HasItem(KeyItemData item)
    {
        return obtainedItems.Contains(item);
    }

    // デバッグ用：取得済みアイテムと個数を確認
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (var it in obtainedItems)
            {
                if (counts.ContainsKey(it.itemName)) counts[it.itemName]++;
                else counts[it.itemName] = 1;
            }

            Debug.Log("=== 取得済みアイテム ===");
            foreach (var pair in counts)
            {
                Debug.Log($"{pair.Key} : {pair.Value}個");
            }
            Debug.Log("=======================");
        }
    }
}
