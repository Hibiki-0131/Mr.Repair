using UnityEngine;

public class ResettableStageController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private RoomBuilder roomBuilder;
    private StageContext stageContext;

    private Vector3 playerStartPos;
    private Quaternion playerStartRot;
    private Rigidbody playerRb;

    // ================================
    // Dependency Injection
    // ================================
    public void SetDependencies(RoomBuilder builder, StageContext context)
    {
        roomBuilder = builder;
        stageContext = context;
    }

    private void Awake()
    {
        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
            playerRb = player.GetComponent<Rigidbody>();
        }
    }

    // ================================
    // Reset API
    // ================================
    public void ResetStage()
    {
        if (roomBuilder == null || stageContext == null)
        {
            Debug.LogError("[ResettableStageController] Dependencies not set", this);
            return;
        }

        // CarryBlock 削除
        foreach (var b in FindObjectsOfType<PushableBlock>())
            Destroy(b.gameObject);

        // Terrain 再構築
        TerrainState terrain = roomBuilder.BuildTerrain();
        stageContext.SetTerrain(terrain);

        // Player リセット
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = playerStartPos;
            playerRb.rotation = playerStartRot;
        }

        Physics.SyncTransforms();
    }
}
