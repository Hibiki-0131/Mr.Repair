using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlace", menuName = "Place")]
public class PlaceData : ScriptableObject
{
    public string placeID;        // アイテム使用時に一致させるID
    public string displayName;    // UIなどで表示したい名前
}
