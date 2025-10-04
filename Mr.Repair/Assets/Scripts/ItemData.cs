using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyItem", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;        // アイテム名
    public Sprite icon;            // アイコン
    public string usablePlaceID;   // 使用可能な場所ID
}
