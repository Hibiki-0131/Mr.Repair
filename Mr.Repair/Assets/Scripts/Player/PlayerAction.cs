using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private bool canInteract = false;
    private bool canUse = false;
    private GameObject currentItemObject;
    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public void Jump()
    {
        Debug.Log("ジャンプ");
    }

    public void Interact()
    {
        if (canInteract && currentItemObject != null)
        {
            ItemInteractable item = currentItemObject.GetComponent<ItemInteractable>();
            if (item != null)
            {
                inventory.ObtainItem(item.itemData);
                Debug.Log($"{item.itemData.itemName} を拾いました");
                Destroy(currentItemObject);
            }
        }
        else
        {
            Debug.Log("近くに拾えるアイテムがありません。");
        }
    }

    public void Use()
    {
        if (canUse && currentItemObject != null)
        {
            PlaceInteractable place = currentItemObject.GetComponent<PlaceInteractable>();
            if (place != null)
            {
                foreach (var item in inventory.GetAllItems())
                {
                    if (item.usablePlaceID == place.PlaceID) // ← 修正！
                    {
                        Debug.Log($"{item.itemName} を {place.PlaceID} で使用しました！");
                        inventory.UseItem(item);
                        place.UseItem(item); // ← 実際の演出呼び出し
                        return;
                    }
                }

                Debug.Log($"この場所 ({place.PlaceID}) では使用できるアイテムがありません。");
            }
        }
        else
        {
            Debug.Log("ここでアイテムは使えません。");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            canInteract = true;
            currentItemObject = other.gameObject;
        }

        if (other.CompareTag("Field"))
        {
            canUse = true;
            currentItemObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            canInteract = false;
            currentItemObject = null;
        }

        if (other.CompareTag("Field"))
        {
            canUse = false;
            currentItemObject = null;
        }
    }
}
