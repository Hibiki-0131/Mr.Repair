using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyItem : MonoBehaviour
{
    public KeyItemData itemData; // このオブジェクトが持つアイテム情報

    public void Interact()
    {
        Debug.Log($"{itemData.itemName} を拾いました。");
        FindObjectOfType<PlayerInventory>().ObtainItem(itemData);
        Destroy(gameObject);
    }
}
