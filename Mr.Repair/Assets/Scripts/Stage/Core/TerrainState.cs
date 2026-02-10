using UnityEngine;

/// <summary>
/// Logical terrain state.
/// CarryBlock が落ちた位置を地面化する責務のみ持つ
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
    /// CarryBlock が静止した位置を地面化する
    /// 条件：下に床があるだけ
    /// </summary>
    public bool TryFillFromCarryBlock(
        Vector3 localPos,
        out Vector3 snappedPos)
    {
        snappedPos = Vector3.zero;

        // -------------------------
        // 1. local → grid
        // -------------------------
        int x = Mathf.FloorToInt(localPos.x / voxelSize);
        int y = Mathf.FloorToInt(localPos.y / voxelSize) - yOffset;
        int z = Mathf.FloorToInt(localPos.z / voxelSize);

        if (!IsInside(x, y, z))
            return false;

        // -------------------------
        // 2. 穴セルのみ
        // -------------------------
        if (csvGrid[x, y, z] != 0)
            return false;

        if (SolidGrid[x, y, z])
            return false;

        // -------------------------
        // ★ 3. 下に床があるだけチェック
        // -------------------------
        int belowY = y - 1;

        if (belowY < 0 || !SolidGrid[x, belowY, z])
            return false;

        // -------------------------
        // 4. 地面化確定
        // -------------------------
        SolidGrid[x, y, z] = true;

        snappedPos = new Vector3(
            (x + 0.5f) * voxelSize,
            (y + yOffset + 0.5f) * voxelSize,
            (z + 0.5f) * voxelSize
        );

        return true;
    }

    // =========================

    private bool IsInside(int x, int y, int z)
    {
        return
            x >= 0 && y >= 0 && z >= 0 &&
            x < SolidGrid.GetLength(0) &&
            y < SolidGrid.GetLength(1) &&
            z < SolidGrid.GetLength(2);
    }
}
