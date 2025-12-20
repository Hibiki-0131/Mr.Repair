using UnityEngine;

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

    public bool TryFillFromCarryBlock(
        Vector3 localPos,
        out Vector3 snappedPos)
    {
        snappedPos = Vector3.zero;

        int x = Mathf.FloorToInt(localPos.x / voxelSize);
        int y = Mathf.FloorToInt(localPos.y / voxelSize) - yOffset;
        int z = Mathf.FloorToInt(localPos.z / voxelSize);

        if (!IsInside(x, y, z))
            return false;

        // °Œó•â‚Å‚È‚¯‚ê‚Î•s‰Â
        if (csvGrid[x, y, z] != 1)
            return false;

        // ‚·‚Å‚É°‚È‚ç•s‰Â
        if (SolidGrid[x, y, z])
            return false;

        SolidGrid[x, y, z] = true;

        snappedPos = new Vector3(
            (x + 0.5f) * voxelSize,
            (y + yOffset + 0.5f) * voxelSize,
            (z + 0.5f) * voxelSize
        );

        return true;
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
