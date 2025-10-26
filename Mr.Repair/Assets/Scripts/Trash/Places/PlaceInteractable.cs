using UnityEngine;

public class PlaceInteractable : MonoBehaviour
{
    [SerializeField] private string placeID;
    public string PlaceID => placeID; // ← プロパティ追加

    public void UseItem(ItemData item)
    {
        if (item == null) return;

        if (item.usablePlaceID == placeID)
        {
            item.effect?.ExecuteEffect(gameObject);
        }
        else
        {
            Debug.Log($"{item.itemName} はこの場所 ({placeID}) では使えません。");
        }
    }
}
