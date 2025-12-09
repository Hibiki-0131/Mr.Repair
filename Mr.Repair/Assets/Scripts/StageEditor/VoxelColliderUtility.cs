using UnityEngine;

public static class VoxelColliderUtility
{
    public static void BuildColliders(Transform contentRoot, bool[,,] solid, float voxelSize, int yOffset)
    {
        foreach (var col in contentRoot.GetComponents<BoxCollider>())
            Object.DestroyImmediate(col);

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
                    if (!solid[x, y, z] || visited[x, y, z]) continue;

                    int maxX = x, maxY = y, maxZ = z;
                    Expand(ref maxX, ref maxZ, ref maxY, solid, visited, x, y, z);

                    CreateCollider(contentRoot, x, maxX, y, maxY, z, maxZ, voxelSize, yOffset);
                }
            }
        }
    }

    private static void Expand(ref int maxX, ref int maxZ, ref int maxY, bool[,,] solid, bool[,,] visited, int x, int y, int z)
    {
        int width = solid.GetLength(0);
        int height = solid.GetLength(1);
        int depth = solid.GetLength(2);

        while (maxX + 1 < width && solid[maxX + 1, y, z] && !visited[maxX + 1, y, z])
            maxX++;

        bool contZ = true;
        while (contZ && maxZ + 1 < depth)
        {
            for (int xi = x; xi <= maxX; xi++)
                if (!solid[xi, y, maxZ + 1] || visited[xi, y, maxZ + 1]) contZ = false;
            if (contZ) maxZ++;
        }

        bool contY = true;
        while (contY && maxY + 1 < height)
        {
            for (int zi = z; zi <= maxZ; zi++)
                for (int xi = x; xi <= maxX; xi++)
                    if (!solid[xi, maxY + 1, zi] || visited[xi, maxY + 1, zi]) contY = false;
            if (contY) maxY++;
        }

        for (int yy = y; yy <= maxY; yy++)
            for (int zz = z; zz <= maxZ; zz++)
                for (int xx = x; xx <= maxX; xx++)
                    visited[xx, yy, zz] = true;
    }

    private static void CreateCollider(Transform parent, int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float voxelSize, int yOffset)
    {
        float sizeX = (maxX - minX + 1) * voxelSize;
        float sizeY = (maxY - minY + 1) * voxelSize;
        float sizeZ = (maxZ - minZ + 1) * voxelSize;

        float cx = ((minX + maxX) / 2f) * voxelSize;
        float cy = ((minY + maxY) / 2f) * voxelSize + yOffset;
        float cz = ((minZ + maxZ) / 2f) * voxelSize;

        var col = parent.gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(cx, cy, cz);
        col.size = new Vector3(sizeX, sizeY, sizeZ);
    }
}
