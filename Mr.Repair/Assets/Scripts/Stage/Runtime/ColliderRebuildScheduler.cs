using UnityEngine;

public class ColliderRebuildScheduler : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private TerrainState terrain;
    [SerializeField] private RoomBuilder owner;

    private bool dirty;

    // ================================
    // Dependency Injection
    // ================================
    public void SetOwner(RoomBuilder builder)
    {
        owner = builder;

        if (contentRoot == null && builder != null)
        {
            contentRoot = builder.ContentRoot;
        }
    }

    public void SetTerrain(TerrainState terrain)
    {
        this.terrain = terrain;
        RequestRebuild();
    }

    // ================================
    // Rebuild Request
    // ================================
    public void RequestRebuild()
    {
        dirty = true;
    }

    private void LateUpdate()
    {
        if (!dirty)
            return;

        dirty = false;

        if (owner == null || terrain == null || contentRoot == null)
        {
            Debug.LogError(
                "[ColliderRebuildScheduler] Missing dependencies",
                this
            );
            return;
        }

        VoxelColliderUtility.BuildColliders(
            contentRoot,
            terrain.SolidGrid,
            owner.VoxelSize,
            owner.YOffset,
            owner
        );
    }
}
