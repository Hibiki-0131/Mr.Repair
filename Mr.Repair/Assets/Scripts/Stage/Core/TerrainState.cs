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

        // -------------------------
        // 1. ローカル座標 → グリッド座標
        // -------------------------
        int x = Mathf.FloorToInt(localPos.x / voxelSize);
        int y = Mathf.FloorToInt(localPos.y / voxelSize) - yOffset;
        int z = Mathf.FloorToInt(localPos.z / voxelSize);

        if (!IsInside(x, y, z))
            return false;

        // -------------------------
        // 2. 穴セルであること
        // -------------------------
        if (csvGrid[x, y, z] != 0)
            return false;

        if (SolidGrid[x, y, z])
            return false;

        // -------------------------
        // 3. 下方向に支えがあること
        // -------------------------
        int belowY = y - 1;
        if (belowY < 0 || !SolidGrid[x, belowY, z])
            return false;

        // -------------------------
        // 4. 四方向の囲まれ判定（重要）
        // -------------------------
        int solidCount = 0;

        if (IsSolid(x - 1, y, z)) solidCount++;
        if (IsSolid(x + 1, y, z)) solidCount++;
        if (IsSolid(x, y, z - 1)) solidCount++;
        if (IsSolid(x, y, z + 1)) solidCount++;

        // 3方向以上に囲まれていなければ地面化しない
        if (solidCount < 3)
            return false;

        // -------------------------
        // 5. 地面として確定
        // -------------------------
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
