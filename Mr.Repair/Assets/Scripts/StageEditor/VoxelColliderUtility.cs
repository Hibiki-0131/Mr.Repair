using UnityEngine;

public static class VoxelColliderUtility
{
    public static void BuildColliders(Transform contentRoot, bool[,,] solid, float voxelSize, int yOffset)
    {
        // 既存コライダー削除（ただし PushableBlock のコライダーは残す）
        foreach (Transform child in contentRoot)
        {
            if (child.GetComponent<PushableBlock>() != null)
                continue; // キャリーブロックは対象外

            var col = child.GetComponent<BoxCollider>();
            if (col != null)
                Object.Destroy(col); // DestroyImmediate禁止
        }

        foreach (var col in contentRoot.GetComponents<BoxCollider>())
            Object.Destroy(col);

        int width = solid.GetLength(0);
        int height = solid.GetLength(1);
        int depth = solid.GetLength(2);

        bool[,,] visited = new bool[width, height, depth];

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!solid[x, y, z] || visited[x, y, z])
                        continue;

                    int maxX = x, maxY = y, maxZ = z;

                    // Expand X
                    int xEnd = x;
                    while (xEnd + 1 < width && solid[xEnd + 1, y, z] && !visited[xEnd + 1, y, z])
                        xEnd++;
                    maxX = xEnd;

                    // Expand Z
                    int zEnd = z;
                    bool okZ = true;
                    while (okZ && zEnd + 1 < depth)
                    {
                        for (int xx = x; xx <= maxX; xx++)
                        {
                            if (!solid[xx, y, zEnd + 1] || visited[xx, y, zEnd + 1])
                                okZ = false;
                        }
                        if (okZ) zEnd++;
                    }
                    maxZ = zEnd;

                    // Expand Y
                    int yEnd = y;
                    bool okY = true;
                    while (okY && yEnd + 1 < height)
                    {
                        for (int zz = z; zz <= maxZ; zz++)
                            for (int xx = x; xx <= maxX; xx++)
                                if (!solid[xx, yEnd + 1, zz] || visited[xx, yEnd + 1, zz])
                                    okY = false;

                        if (okY) yEnd++;
                    }
                    maxY = yEnd;

                    // Mark visited
                    for (int yy = y; yy <= maxY; yy++)
                        for (int zz = z; zz <= maxZ; zz++)
                            for (int xx = x; xx <= maxX; xx++)
                                visited[xx, yy, zz] = true;

                    // Create collider
                    CreateCollider(contentRoot, x, maxX, y, maxY, z, maxZ, voxelSize, yOffset);
                }
            }
        }
    }

    private static void CreateCollider(
        Transform parent,
        int minX, int maxX,
        int minY, int maxY,
        int minZ, int maxZ,
        float voxelSize, int yOffset)
    {
        var col = parent.gameObject.AddComponent<BoxCollider>();

        float sizeX = (maxX - minX + 1) * voxelSize;
        float sizeY = (maxY - minY + 1) * voxelSize;
        float sizeZ = (maxZ - minZ + 1) * voxelSize;

        float centerX = (minX + maxX) * 0.5f * voxelSize;
        float centerY = (minY + maxY) * 0.5f * voxelSize + yOffset;
        float centerZ = (minZ + maxZ) * 0.5f * voxelSize;

        col.center = new Vector3(centerX, centerY, centerZ);
        col.size = new Vector3(sizeX, sizeY, sizeZ);
    }
}
