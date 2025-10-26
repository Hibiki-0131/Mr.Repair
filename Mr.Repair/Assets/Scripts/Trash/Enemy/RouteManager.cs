using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [Header("巡回経路リスト（各要素をEmptyオブジェクトにして子にポイントを置く）")]
    public Transform[] routeParents;

    public Transform[][] GetAllRoutes()
    {
        if (routeParents == null || routeParents.Length == 0) return new Transform[0][];

        Transform[][] routes = new Transform[routeParents.Length][];
        for (int i = 0; i < routeParents.Length; i++)
        {
            Transform parent = routeParents[i];
            routes[i] = new Transform[parent.childCount];
            for (int j = 0; j < parent.childCount; j++)
            {
                routes[i][j] = parent.GetChild(j);
            }
        }

        return routes;
    }
}
