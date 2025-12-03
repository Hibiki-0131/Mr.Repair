using UnityEngine;

public static class VoxelColliderUtility
{
    public static void BuildColliders(Transform contentRoot, bool[,,] solid, float voxelSize, int yOffset)
    {
        // 既存のコライダー削除
        foreach (var col in contentRoot.GetComponents<BoxCollider>())
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Object.DestroyImmediate(col);
            else
                Object.Destroy(col);
#else
            Object.Destroy(col);
#endif
        }

        // 子の BoxCollider 削除
        foreach (Transform child in contentRoot)
        {
            var childCol = child.GetComponent<BoxCollider>();
            if (childCol != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(childCol);
                else
                    Object.Destroy(childCol);
#else
                Object.Destroy(childCol);
#endif
            }
        }

        // 以下、普通にコライダー生成
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

                    int maxX = x;
                    int maxY = y;
                    int maxZ = z;

                    int xEnd = x;
                    while (xEnd + 1 < width && solid[xEnd + 1, y, z] && !visited[xEnd + 1, y, z])
                        xEnd++;
                    maxX = xEnd;

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

                    for (int yy = y; yy <= maxY; yy++)
                        for (int zz = z; zz <= maxZ; zz++)
                            for (int xx = x; xx <= maxX; xx++)
                                visited[xx, yy, zz] = true;

                    CreateCollider(contentRoot, x, maxX, y, maxY, z, maxZ, voxelSize, yOffset);
                }
            }
        }
    }

    private static void CreateCollider(Transform parent, int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float voxelSize, int yOffset)
    {
        float sizeX = (maxX - minX + 1) * voxelSize;
        float sizeY = (maxY - minY + 1) * voxelSize;
        float sizeZ = (maxZ - minZ + 1) * voxelSize;

        float centerX = ((minX + maxX) / 2f) * voxelSize;
        float centerY = ((minY + maxY) / 2f) * voxelSize + yOffset;
        float centerZ = ((minZ + maxZ) / 2f) * voxelSize;

        var col = parent.gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(centerX, centerY, centerZ);
        col.size = new Vector3(sizeX, sizeY, sizeZ);
    }
}
