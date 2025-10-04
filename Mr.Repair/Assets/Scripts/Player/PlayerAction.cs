using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private bool canInteract = false;
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
                inventory.ObtainItem(item.itemData); // ここでUIも更新される
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
        Debug.Log("アイテムをつかった");
    }

    public void Repair()
    {
        Debug.Log("修理を施した");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            canInteract = true;
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
    }
}
