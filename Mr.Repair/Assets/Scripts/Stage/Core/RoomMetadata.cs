using UnityEngine;

[CreateAssetMenu(menuName = "Stage/RoomMetadata")]
public class RoomMetadata : ScriptableObject
{
    [Header("Room Info")]
    public string roomName;
    [TextArea(2, 5)]
    public string description;

    [Header("CSV Layout (11 layers)")]
    public TextAsset roomCsv;

    [Header("Visual Settings")]
    public Color roomColor = Color.white;
    public Color floorColor = Color.white; // ★追加：床(csv=1)のデフォルト色

    [Header("Room Objects")]
    public GameObject[] traps;
    public GameObject[] decorations;

    [Header("Settings")]
    public bool autoBuildOnLoad = true;
}