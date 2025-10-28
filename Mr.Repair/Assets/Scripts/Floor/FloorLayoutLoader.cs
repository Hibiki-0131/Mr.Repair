using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class FloorLayoutLoader : MonoBehaviour
{
    [Header("JSON設定")]
    [Tooltip("JSONファイルをドラッグ＆ドロップで設定")]
    [SerializeField] private TextAsset jsonFile;  // ← 変更点！

    [Header("生成設定")]
    [SerializeField] private GameObject tilePrefab;

    void Start()
    {
        if (jsonFile == null)
        {
            Debug.LogError("[FloorLayoutLoader] JSONファイルが未設定です（InspectorにTextAssetをドラッグしてください）");
            return;
        }

        if (tilePrefab == null)
        {
            Debug.LogError("[FloorLayoutLoader] Tile Prefab が未設定です");
            return;
        }

        LoadAndGenerate();
    }

    void LoadAndGenerate()
    {
        // 直接TextAssetの中身を使う
        string json = jsonFile.text;

        FloorLayoutData data = JsonConvert.DeserializeObject<FloorLayoutData>(json);

        if (data == null || data.layout == null)
        {
            Debug.LogError("[FloorLayoutLoader] JSONの読み込みに失敗しました。");
            return;
        }

        for (int z = 0; z < data.height; z++)
        {
            for (int x = 0; x < data.width; x++)
            {
                if (data.layout[z][x] == 1)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                }
            }
        }

        Debug.Log("床生成完了: " + jsonFile.name);
    }
}
