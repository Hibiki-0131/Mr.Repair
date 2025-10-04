using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemInteractable : MonoBehaviour
{
    public KeyItemData itemData; // このオブジェクトのアイテム情報

    private void Reset()
    {
        // Colliderはトリガーにする
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }
}
