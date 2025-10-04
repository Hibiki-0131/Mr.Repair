using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListUI : MonoBehaviour
{
    public static ItemListUI Instance;

    [SerializeField] private Transform panel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Sprite unknownIcon;

    // 各アイテムごとに複数スロット（Image）を管理
    private Dictionary<ItemData, List<Image>> slotImages = new Dictionary<ItemData, List<Image>>();

    private void Awake() => Instance = this;

    // 全アイテムスロット初期化
    public void InitializeSlots(List<ItemData> allItems)
    {
        slotImages.Clear();

        foreach (var item in allItems)
        {
            GameObject slot = Instantiate(slotPrefab, panel);
            Image img = slot.GetComponent<Image>();
            img.sprite = unknownIcon;

            if (!slotImages.ContainsKey(item))
                slotImages[item] = new List<Image>();

            slotImages[item].Add(img);
        }
    }

    /// <summary>
    /// アイテム取得 or 使用に応じてスロットを更新
    /// </summary>
    /// <param name="item">対象のアイテム</param>
    /// <param name="isObtained">true: 取得 / false: 使用</param>
    public void UpdateSlot(ItemData item, bool isObtained)
    {
        if (!slotImages.ContainsKey(item)) return;

        if (isObtained)
        {
            // 未取得スロットを1つ更新
            foreach (var img in slotImages[item])
            {
                if (img.sprite == unknownIcon)
                {
                    img.sprite = item.icon;
                    break;
                }
            }
        }
        else
        {
            // 取得済みスロットを1つ減らす（最後にあるアイコンを?に戻す）
            for (int i = slotImages[item].Count - 1; i >= 0; i--)
            {
                if (slotImages[item][i].sprite == item.icon)
                {
                    slotImages[item][i].sprite = unknownIcon;
                    break;
                }
            }
        }
    }
}
