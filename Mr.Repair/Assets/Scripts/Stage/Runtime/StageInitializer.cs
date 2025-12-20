using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RoomBuilder roomBuilder;
    [SerializeField] private StageContext stageContext;

    private bool initialized;

    // EditorWindow Ç©ÇÁåƒÇŒÇÍÇÈ
    public void SetDependencies(
        RoomBuilder builder,
        StageContext context
    )
    {
        roomBuilder = builder;
        stageContext = context;
    }

    private void Awake()
    {
        // Play éûÇ… Inspector Ç…écÇ¡ÇƒÇ¢Ç»ÇØÇÍÇŒé©ìÆíTçı
        if (roomBuilder == null)
            roomBuilder = GetComponent<RoomBuilder>();

        if (stageContext == null)
            stageContext = GetComponent<StageContext>();
    }

    private void Start()
    {
        if (initialized)
            return;

        if (roomBuilder == null || stageContext == null)
        {
            Debug.LogError("[StageInitializer] Dependencies not set", this);
            return;
        }

        InitializeStage();
        initialized = true;
    }

    private void InitializeStage()
    {
        // Terrain ç\íz
        TerrainState terrain = roomBuilder.BuildTerrain();
        stageContext.SetTerrain(terrain);

        // carryblock ê∂ê¨
        GameObject carryPrefab = BlockFactory.GetPrefab('3');
        if (carryPrefab == null)
        {
            Debug.LogError("[StageInitializer] CarryBlock prefab not found", this);
            return;
        }

        foreach (Vector3 pos in roomBuilder.GetCarryBlockPositions())
        {
            Instantiate(
                carryPrefab,
                pos,
                Quaternion.identity,
                roomBuilder.ContentRoot
            );
        }
    }
}
