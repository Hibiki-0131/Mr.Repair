using UnityEngine;

public class ColliderRebuildScheduler : MonoBehaviour
{
    [Header("Dependencies (auto-resolved if empty)")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private TerrainState terrain;
    [SerializeField] private RoomBuilder owner;

    private bool dirty;
    private bool dependencyResolved;

    // ================================
    // Unity Lifecycle
    // ================================
    private void Awake()
    {
        ResolveDependencies();
    }

    private void LateUpdate()
    {
        // ˆË‘¶‚ª‚Ü‚¾‘µ‚Á‚Ä‚¢‚È‚¯‚ê‚ÎÄs
        if (!dependencyResolved)
        {
            ResolveDependencies();
            return;
        }

        if (!dirty)
            return;

        dirty = false;

        VoxelColliderUtility.BuildColliders(
            contentRoot,
            terrain.SolidGrid,
            owner.VoxelSize,
            owner.YOffset,
            owner
        );
    }

    // ================================
    // Dependency Resolution
    // ================================
    private void ResolveDependencies()
    {
        // owner ©“®æ“¾
        if (owner == null)
            owner = GetComponentInParent<RoomBuilder>();

        // contentRoot ©“®æ“¾
        if (contentRoot == null && owner != null)
            contentRoot = owner.ContentRoot;

        dependencyResolved =
            owner != null &&
            terrain != null &&
            contentRoot != null;

        if (!dependencyResolved)
        {
            Debug.LogWarning(
                "[ColliderRebuildScheduler] Waiting for dependencies",
                this
            );
            return;
        }

        // ‘µ‚Á‚½uŠÔ‚Éˆê“x Rebuild
        RequestRebuild();
    }

    // ================================
    // External API
    // ================================
    public void SetOwner(RoomBuilder builder)
    {
        owner = builder;
        contentRoot = builder != null ? builder.ContentRoot : null;
        ResolveDependencies();
    }

    public void SetTerrain(TerrainState terrain)
    {
        this.terrain = terrain;
        ResolveDependencies();
    }

    public void RequestRebuild()
    {
        dirty = true;
    }
}
