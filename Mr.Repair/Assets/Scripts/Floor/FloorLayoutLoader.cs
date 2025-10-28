using UnityEngine;
using System.IO;

[System.Serializable]
public class FloorLayoutData
{
    public int width;
    public int height;
    public int[][] layout;
}

public class FloorLayoutLoader : MonoBehaviour
{
    [SerializeField] private string jsonFileName = "roomA_floor.json";
    [SerializeField] private GameObject tilePrefab;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            FloorLayoutData data = JsonUtility.FromJson<FloorLayoutData>(json);

            GenerateFloor(data);
        }
        else
        {
            Debug.LogError($"JSONƒtƒ@ƒCƒ‹‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ: {path}");
        }
    }

    void GenerateFloor(FloorLayoutData data)
    {
        for (int z = 0; z < data.height; z++)
        {
            for (int x = 0; x < data.width; x++)
            {
                if (data.layout[z][x] == 1)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}
