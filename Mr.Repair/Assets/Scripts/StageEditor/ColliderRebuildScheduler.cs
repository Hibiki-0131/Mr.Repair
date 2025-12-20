using UnityEngine;

public class ColliderRebuildScheduler : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RoomBuilder owner;   // š’Ç‰Á
    [SerializeField] private TerrainState terrain;
    [SerializeField] private float voxelSize;
    [SerializeField] private int yOffset;

    private bool dirty;

    public void RequestRebuild()
    {
        dirty = true;
    }

    private void LateUpdate()
    {
        if (!dirty) return;
        dirty = false;

        VoxelColliderUtility.BuildColliders(
            contentRoot,
            terrain.SolidGrid,
            voxelSize,
            yOffset,
            owner            // š’Ç‰Á
        );
    }

    public void SetTerrain(TerrainState terrain)
    {
        this.terrain = terrain;
        RequestRebuild(); // Reset ’¼Œã‚É•K‚¸Ä\’z
    }
}

