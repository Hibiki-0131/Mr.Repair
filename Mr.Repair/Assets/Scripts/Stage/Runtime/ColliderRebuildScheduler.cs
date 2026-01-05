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
        // 依存がまだ揃っていなければ再試行
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
        // owner 自動取得
        if (owner == null)
            owner = GetComponentInParent<RoomBuilder>();

        // contentRoot 自動取得
        if (contentRoot == null && owner != null)
            contentRoot = owner.ContentRoot;

        // terrain は外部注入が基本だが、
        // 同一 GameObject にあれば補完
        if (terrain == null)
            terrain = GetComponent<TerrainState>();

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

        // 揃った瞬間に一度 Rebuild
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
