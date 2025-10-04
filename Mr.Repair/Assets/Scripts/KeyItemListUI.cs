using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyItemListUI : MonoBehaviour
{
    public static KeyItemListUI Instance;

    [SerializeField] private Transform panel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Sprite unknownIcon;

    // KeyItemData 1種類に対して複数のスロットを持てるようにする
    private Dictionary<KeyItemData, List<Image>> slotImages = new Dictionary<KeyItemData, List<Image>>();

    private void Awake() => Instance = this;

    // 全アイテムスロット初期化
    public void InitializeSlots(List<KeyItemData> allItems)
    {
        slotImages.Clear();

        foreach (var item in allItems)
        {
            // スロットの数を1つにして初期化
            GameObject slot = Instantiate(slotPrefab, panel);
            Image img = slot.GetComponent<Image>();
            img.sprite = unknownIcon;

            if (!slotImages.ContainsKey(item))
                slotImages[item] = new List<Image>();

            slotImages[item].Add(img);
        }
    }

    // アイテム取得時に呼ぶ
    public void UpdateSlot(KeyItemData item, bool isObtained)
    {
        if (slotImages.ContainsKey(item))
        {
            // 未取得スロットのうち1つだけ更新
            foreach (var img in slotImages[item])
            {
                if (img.sprite == unknownIcon)
                {
                    img.sprite = item.icon;
                    break;  // 1つだけ更新
                }
            }
        }
    }
}
