using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> allItems;  // マップ上の全アイテム
    private List<ItemData> obtainedItems = new List<ItemData>();

    private void Start()
    {
        // UI にアイテムリストを渡して初期化
        ItemListUI.Instance.InitializeSlots(allItems);
    }

    // アイテム取得
    public void ObtainItem(ItemData item)
    {
        obtainedItems.Add(item);  // ←重複チェックを削除
        Debug.Log($"{item.itemName} を取得しました。");

        // UI 更新
        ItemListUI.Instance.UpdateSlot(item, true);
    }

    // アイテム使用
    public void UseItem(ItemData item)
    {
        if (obtainedItems.Contains(item))
        {
            obtainedItems.Remove(item);  // 1つだけ削除
            Debug.Log($"{item.itemName} を使用しました。");

            // UI 更新（元の？アイコンに戻す）
            ItemListUI.Instance.UpdateSlot(item, false);
        }
    }

    // 取得済みか確認
    public bool HasItem(ItemData item)
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

    public List<ItemData> GetAllItems()
    {
        return obtainedItems;
    }

}
