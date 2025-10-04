using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeyItem", menuName = "KeyItem")]
public class KeyItemData : ScriptableObject
{
    public string itemName;        // アイテム名
    public Sprite icon;            // アイコン
    public string usablePlaceID;   // 使用可能な場所ID
}
