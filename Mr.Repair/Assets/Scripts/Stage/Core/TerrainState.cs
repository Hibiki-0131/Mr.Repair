using UnityEngine;

/// <summary>
/// Logical terrain state.
/// csvGrid  : original definition (potential ground)
/// SolidGrid : current solid state
/// </summary>
public class TerrainState
{
    private readonly int[,,] csvGrid;
    public bool[,,] SolidGrid { get; }

    private readonly float voxelSize;
    private readonly int yOffset;

    public TerrainState(
        int[,,] csv,
        bool[,,] solid,
        float voxelSize,
        int yOffset)
    {
        csvGrid = csv;
        SolidGrid = solid;
        this.voxelSize = voxelSize;
        this.yOffset = yOffset;
    }

    /// <summary>
    /// Try to convert a hole cell into ground when a CarryBlock settles.
    /// Ground is created only when the cell becomes connected
    /// to existing ground.
    /// </summary>
    public bool TryFillFromCarryBlock(
        Vector3 localPos,
        out Vector3 snappedPos)
    {
        snappedPos = Vector3.zero;

        // Convert local position to grid position
        int x = Mathf.FloorToInt(localPos.x / voxelSize);
        int y = Mathf.FloorToInt(localPos.y / voxelSize) - yOffset;
        int z = Mathf.FloorToInt(localPos.z / voxelSize);

        if (!IsInside(x, y, z))
            return false;

        // Condition 1: must be a hole (csv = 0)
        if (csvGrid[x, y, z] != 0)
            return false;

        if (SolidGrid[x, y, z])
            return false;

        // Condition 2: must be supported from below
        int belowY = y - 1;
        if (belowY < 0 || !SolidGrid[x, belowY, z])
            return false;

        // Condition 3: must connect to existing ground
        bool connected =
            IsSolid(x - 1, y, z) ||
            IsSolid(x + 1, y, z) ||
            IsSolid(x, y, z - 1) ||
            IsSolid(x, y, z + 1);

        if (!connected)
            return false;

        // Confirm ground
        SolidGrid[x, y, z] = true;

        snappedPos = new Vector3(
            (x + 0.5f) * voxelSize,
            (y + yOffset + 0.5f) * voxelSize,
            (z + 0.5f) * voxelSize
        );

        return true;
    }

    // -------------------------
    // Internal helpers
    // -------------------------

    private bool IsSolid(int x, int y, int z)
    {
        if (!IsInside(x, y, z))
            return false;

        return SolidGrid[x, y, z];
    }

    private bool IsInside(int x, int y, int z)
    {
        return
            x >= 0 && y >= 0 && z >= 0 &&
            x < SolidGrid.GetLength(0) &&
            y < SolidGrid.GetLength(1) &&
            z < SolidGrid.GetLength(2);
    }
}
