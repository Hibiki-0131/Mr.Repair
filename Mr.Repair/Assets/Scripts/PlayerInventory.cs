using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<KeyItemData> obtainedItems = new List<KeyItemData>();
    [SerializeField] private List<KeyItemData> allKeyItems; // 全アイテムリスト（Inspectorで設定）

    private void Start()
    {
        // UIスロットを初期化
        KeyItemListUI.Instance.InitializeSlots(allKeyItems);
    }

    public void ObtainItem(KeyItemData item)
    {
        if (!obtainedItems.Contains(item))
        {
            obtainedItems.Add(item);
            Debug.Log($"{item.itemName} を取得しました。");

            // UI更新
            KeyItemListUI.Instance.UpdateSlot(item, true);
        }
    }

    public bool HasItem(KeyItemData item)
    {
        return obtainedItems.Contains(item);
    }

    // デバッグ表示（Qキー）
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (obtainedItems.Count == 0)
                Debug.Log("取得済みアイテムはありません。");
            else
            {
                Debug.Log(" 取得済みアイテム一覧：");
                foreach (var i in obtainedItems)
                    Debug.Log($" - {i.itemName}");
            }
        }
    }
}
