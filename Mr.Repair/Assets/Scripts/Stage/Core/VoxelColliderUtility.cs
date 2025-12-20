using UnityEngine;

public static class VoxelColliderUtility
{
    public static void BuildColliders(
        Transform contentRoot,
        bool[,,] solid,
        float voxelSize,
        int yOffset,
        RoomBuilder owner)
    {
        if (contentRoot == null || solid == null)
            return;

        // ============================
        // ContentRoot に付いている Collider を全削除
        // ============================
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

        // ============================
        // 子オブジェクトの Collider を削除
        // ※ PushableBlock の Collider は残す
        // ============================
        foreach (Transform child in contentRoot)
        {
            if (child.GetComponent<PushableBlock>() != null)
                continue;

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

        int width = solid.GetLength(0);
        int height = solid.GetLength(1);
        int depth = solid.GetLength(2);

        bool[,,] visited = new bool[width, height, depth];

        // ============================
        // Greedy Merge（X → Z → Y）
        // ============================
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!solid[x, y, z] || visited[x, y, z])
                        continue;

                    int maxX = x;
                    while (maxX + 1 < width &&
                           solid[maxX + 1, y, z] &&
                           !visited[maxX + 1, y, z])
                        maxX++;

                    int maxZ = z;
                    bool canExpandZ = true;
                    while (canExpandZ && maxZ + 1 < depth)
                    {
                        for (int xi = x; xi <= maxX; xi++)
                        {
                            if (!solid[xi, y, maxZ + 1] ||
                                visited[xi, y, maxZ + 1])
                            {
                                canExpandZ = false;
                                break;
                            }
                        }
                        if (canExpandZ)
                            maxZ++;
                    }

                    int maxY = y;
                    bool canExpandY = true;
                    while (canExpandY && maxY + 1 < height)
                    {
                        for (int zi = z; zi <= maxZ; zi++)
                        {
                            for (int xi = x; xi <= maxX; xi++)
                            {
                                if (!solid[xi, maxY + 1, zi] ||
                                    visited[xi, maxY + 1, zi])
                                {
                                    canExpandY = false;
                                    break;
                                }
                            }
                            if (!canExpandY)
                                break;
                        }
                        if (canExpandY)
                            maxY++;
                    }

                    // visited マーク
                    for (int yy = y; yy <= maxY; yy++)
                        for (int zz = z; zz <= maxZ; zz++)
                            for (int xx = x; xx <= maxX; xx++)
                                visited[xx, yy, zz] = true;

                    CreateCollider(
                        contentRoot,
                        x, maxX,
                        y, maxY,
                        z, maxZ,
                        voxelSize,
                        yOffset
                    );
                }
            }
        }

        // ============================
        // RoomColliderOwner を contentRoot に 1 つだけ
        // ============================
        var ownerTag = contentRoot.GetComponent<RoomColliderOwner>();
        if (ownerTag == null)
            ownerTag = contentRoot.gameObject.AddComponent<RoomColliderOwner>();

        ownerTag.Owner = owner;
    }

    // ============================
    // Collider 生成（contentRoot ローカル基準）
    // ============================
    private static void CreateCollider(
        Transform parent,
        int minX, int maxX,
        int minY, int maxY,
        int minZ, int maxZ,
        float voxelSize,
        int yOffset)
    {
        float sizeX = (maxX - minX + 1) * voxelSize;
        float sizeY = (maxY - minY + 1) * voxelSize;
        float sizeZ = (maxZ - minZ + 1) * voxelSize;

        // ★ RoomBuilder の mesh 生成と完全一致するセル中心
        float centerX = (minX + maxX + 1) * 0.5f * voxelSize;
        float centerY = ((minY + maxY + 1) * 0.5f + yOffset) * voxelSize;
        float centerZ = (minZ + maxZ + 1) * 0.5f * voxelSize;

        var col = parent.gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(centerX, centerY, centerZ);
        col.size = new Vector3(sizeX, sizeY, sizeZ);
    }
}
