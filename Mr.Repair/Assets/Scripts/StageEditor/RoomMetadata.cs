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

    [Header("Visual")]
    public Color roomColor = Color.white;

    [Header("Room Objects")]
    public GameObject[] traps;
    public GameObject[] decorations;

    [Header("Settings")]
    public bool autoBuildOnLoad = true;
}
