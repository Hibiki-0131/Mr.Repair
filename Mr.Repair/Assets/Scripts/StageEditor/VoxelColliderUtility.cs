using UnityEngine;

public static class VoxelColliderUtility
{
    /// <summary>
    /// solid[x,y,z] に基づいて、contentRoot に最小個数の BoxCollider を生成する。
    /// </summary>
    public static void BuildColliders(Transform contentRoot, bool[,,] solid, float voxelSize, int yOffset)
    {
        // 既存コライダーを削除
        foreach (var col in contentRoot.GetComponents<BoxCollider>())
        {
            Object.DestroyImmediate(col);
        }

        // 子の BoxCollider も削除（Tile_Floor に付いている場合）
        foreach (Transform child in contentRoot)
        {
            var childCol = child.GetComponent<BoxCollider>();
            if (childCol != null)
                Object.DestroyImmediate(childCol);
        }

        int width = solid.GetLength(0); // x
        int height = solid.GetLength(1); // y
        int depth = solid.GetLength(2); // z

        bool[,,] visited = new bool[width, height, depth];

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!solid[x, y, z] || visited[x, y, z])
                        continue;

                    // ここを起点に最大の直方体を greedy に拡張
                    int maxX = x;
                    int maxY = y;
                    int maxZ = z;

                    // X方向にどこまで広げられるか
                    int xEnd = x;
                    while (xEnd + 1 < width && CanExpandX(solid, visited, xEnd + 1, y, z))
                        xEnd++;
                    maxX = xEnd;

                    // Z方向に拡張
                    int zEnd = z;
                    bool canExpandZ = true;
                    while (canExpandZ && zEnd + 1 < depth)
                    {
                        for (int xi = x; xi <= maxX; xi++)
                        {
                            if (!solid[xi, y, zEnd + 1] || visited[xi, y, zEnd + 1])
                            {
                                canExpandZ = false;
                                break;
                            }
                        }
                        if (canExpandZ) zEnd++;
                    }
                    maxZ = zEnd;

                    // Y方向に拡張
                    int yEnd = y;
                    bool canExpandY = true;
                    while (canExpandY && yEnd + 1 < height)
                    {
                        for (int zi = z; zi <= maxZ; zi++)
                        {
                            for (int xi = x; xi <= maxX; xi++)
                            {
                                if (!solid[xi, yEnd + 1, zi] || visited[xi, yEnd + 1, zi])
                                {
                                    canExpandY = false;
                                    break;
                                }
                            }
                            if (!canExpandY) break;
                        }
                        if (canExpandY) yEnd++;
                    }
                    maxY = yEnd;

                    // この直方体領域を visited にマーク
                    for (int yy = y; yy <= maxY; yy++)
                        for (int zz = z; zz <= maxZ; zz++)
                            for (int xx = x; xx <= maxX; xx++)
                                visited[xx, yy, zz] = true;

                    // コライダー生成
                    CreateCollider(contentRoot, x, maxX, y, maxY, z, maxZ, voxelSize, yOffset);
                }
            }
        }
    }

    private static bool CanExpandX(bool[,,] solid, bool[,,] visited, int x, int y, int z)
    {
        // y,z 固定で x の列をチェック
        return solid[x, y, z] && !visited[x, y, z];
    }

    private static void CreateCollider(
    Transform parent,
    int minX, int maxX,
    int minY, int maxY,
    int minZ, int maxZ,
    float voxelSize, int yOffset)
    {
        float sizeX = (maxX - minX + 1) * voxelSize;
        float sizeY = (maxY - minY + 1) * voxelSize;
        float sizeZ = (maxZ - minZ + 1) * voxelSize;

        // ★中心座標の修正版（ブロック中心に揃える）
        float centerX = ((minX + maxX) * 0.5f) * voxelSize;
        float centerY = ((minY + maxY) * 0.5f) * voxelSize + yOffset;
        float centerZ = ((minZ + maxZ) * 0.5f) * voxelSize;

        BoxCollider col = parent.gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(centerX, centerY, centerZ);
        col.size = new Vector3(sizeX, sizeY, sizeZ);
    }

}
