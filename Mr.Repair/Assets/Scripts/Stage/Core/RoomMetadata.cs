using UnityEngine;

[CreateAssetMenu(menuName = "Stage/RoomMetadata")]
public class RoomMetadata : ScriptableObject
{
    [Header("Room Info")]
    public string roomName;
    [TextArea(2, 5)]
    public string description;

    [Header("CSV Files")]
    public TextAsset roomCsv;    // 地形構造用 (1:床, 2:ゴール, 3:ブロック)
    public TextAsset colorCsv;   // 追加：色指定用 (0:通常, 1以上:パレット参照)

    [Header("Visual Settings")]
    public Color defaultFloorColor = Color.white; // color.csvが0の時の色
    public Color[] floorPalette; // 追加：color.csvの数値(1, 2...)に対応する色

    [Header("Room Objects")]
    public GameObject[] traps;
    public GameObject[] decorations;

    [Header("Settings")]
    public bool autoBuildOnLoad = true;
}