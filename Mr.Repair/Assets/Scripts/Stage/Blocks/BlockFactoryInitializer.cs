using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BlockFactoryInitializer : MonoBehaviour
{
    public GameObject floorPrefab;      // csv=1
    public GameObject goalPrefab;       // csv=2
    public GameObject carryBlockPrefab; // csv=3

    private void Awake() => Init();
    private void OnEnable() => Init();

    private void Init()
    {
        BlockFactory.Initialize(new Dictionary<char, GameObject>()
        {
            { '1', floorPrefab },
            { '2', goalPrefab },
            { '3', carryBlockPrefab },
            { '0', null },
        });
    }
}
