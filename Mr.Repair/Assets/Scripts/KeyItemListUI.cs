using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyItemListUI : MonoBehaviour
{
    public static KeyItemListUI Instance;

    [SerializeField] private Transform panel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Sprite unknownIcon;

    private List<Image> slotImages = new List<Image>();
    private List<KeyItemData> itemOrder = new List<KeyItemData>(); // 対応関係保持用

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeSlots(List<KeyItemData> allItems)
    {
        // 一度リセット
        foreach (Transform child in panel)
            Destroy(child.gameObject);

        slotImages.Clear();
        itemOrder.Clear();

        // 全アイテムぶんスロット生成
        foreach (var item in allItems)
        {
            GameObject slot = Instantiate(slotPrefab, panel);
            Image img = slot.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = unknownIcon;
                slotImages.Add(img);
                itemOrder.Add(item);
            }
            else
            {
                Debug.LogError("slotPrefab に Image コンポーネントがありません。");
            }
        }
    }

    public void UpdateSlot(KeyItemData item, bool isObtained)
    {
        int index = itemOrder.IndexOf(item);
        if (index >= 0 && index < slotImages.Count)
        {
            slotImages[index].sprite = isObtained ? item.icon : unknownIcon;
        }
        else
        {
            Debug.LogWarning($"{item.name} に対応するスロットが見つかりません。");
        }
    }
}
