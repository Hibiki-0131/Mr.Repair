using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int width = 12;
    [SerializeField] private int height = 12;

    // マップ定義（0 = 穴, 1 = 床）
    private int[,] floorLayout = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,0,0,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1}
    };

    void Start()
    {
        GenerateFloor();
    }

    void GenerateFloor()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (floorLayout[z, x] == 1)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}
