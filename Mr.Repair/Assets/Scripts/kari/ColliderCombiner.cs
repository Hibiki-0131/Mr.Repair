using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 子オブジェクトの Collider を親直下へ集約（複合コライダー化）するユーティリティ。
/// - 親に Rigidbody が1つある構成を想定（親RB + 子Colliders = Compound）
/// - Combine: 子の形状/向きを保った子を "CombinedColliders" 配下に作成して同等Colliderを複製
/// - Clear: 生成したものをすべて削除
/// </summary>
[ExecuteAlways]
public class ColliderCombiner : MonoBehaviour
{
    [Header("実行タイミング")]
    [Tooltip("Startで自動実行するか（エディタ再生/ビルド時）")]
    public bool combineOnStart = false;

    [Header("対象とコピー設定")]
    [Tooltip("isTrigger のコライダーも対象に含める")]
    public bool includeTriggers = true;

    [Tooltip("PhysicMaterial / Center / Size などプロパティをコピー")]
    public bool copyPhysicMaterial = true;

    [Tooltip("元の Collider を無効化する（DestroyOriginal と併用しないこと）")]
    public bool disableOriginals = false;

    [Tooltip("元の Collider を削除する（無効化より強い）。実行前にバックアップ推奨")]
    public bool destroyOriginals = false;

    [Header("ボックスの簡易マージ（任意）")]
    [Tooltip("BoxCollider 同士をグリッドで隣接判定してマージ（向きが同一の軸整列ボックスのみ）")]
    public bool tryMergeAdjacentBoxes = false;

    [Tooltip("グリッド単位（ボックスサイズや配置がこの倍数に揃っている場合に有効）")]
    public float gridSize = 1f;

    private const string kRootName = "CombinedColliders";
    private readonly List<GameObject> generated = new();

    void Start()
    {
        if (Application.isPlaying && combineOnStart)
        {
            Combine();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Combine (エディタ)")]
    private void CombineInEditor()
    {
        Combine();
    }

    [ContextMenu("Clear (エディタ)")]
    private void ClearInEditor()
    {
        Clear();
    }
#endif

    /// <summary>
    /// 生成済みの CombinedColliders 配下を削除
    /// </summary>
    public void Clear()
    {
        var root = transform.Find(kRootName);
        if (root != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.RegisterFullObjectHierarchyUndo(root.gameObject, "Clear Combined Colliders");
                DestroyImmediate(root.gameObject);
            }
            else
#endif
            {
                Destroy(root.gameObject);
            }
        }
        generated.Clear();
    }

    /// <summary>
    /// 子の Collider を集約して複製
    /// </summary>
    public void Combine()
    {
        Clear();

        // 対象コライダーの収集（自分は除外）
        var all = GetComponentsInChildren<Collider>(includeInactive: true);
        List<Collider> targets = new();
        foreach (var c in all)
        {
            if (c.transform == transform) continue; // 自分に付いたものはスキップ
            if (!includeTriggers && c.isTrigger) continue;
            targets.Add(c);
        }

        // ルート作成
        Transform root = new GameObject(kRootName).transform;
        root.SetParent(transform, worldPositionStays: false);

        // まず全コライダーを1:1で複製
        foreach (var src in targets)
        {
            GameObject dup = CreateDuplicatedColliderObject(root, src);
            generated.Add(dup);
        }

        // 任意：Box の簡易マージ
        if (tryMergeAdjacentBoxes)
        {
            TryMergeBoxesUnder(root, gridSize);
        }

        // 元をどうするか
        foreach (var src in targets)
        {
            if (destroyOriginals)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(src);
                else Destroy(src);
#else
                Destroy(src);
#endif
            }
            else if (disableOriginals)
            {
                src.enabled = false;
            }
        }
    }

    private GameObject CreateDuplicatedColliderObject(Transform combinedRoot, Collider src)
    {
        // ワールド姿勢を保持したまま、親直下に子を作る
        GameObject go = new GameObject($"Combined_{src.GetType().Name}_{src.transform.name}");
#if UNITY_EDITOR
        if (!Application.isPlaying) Undo.RegisterCreatedObjectUndo(go, "Create Combined Collider");
#endif
        var t = go.transform;
        t.SetPositionAndRotation(src.transform.position, src.transform.rotation);
        t.localScale = src.transform.lossyScale;
        t.SetParent(combinedRoot, worldPositionStays: true);

        // 形状コピー
        if (src is BoxCollider box)
        {
            var dst = go.AddComponent<BoxCollider>();
            CopyCommon(dst, src);
            dst.center = box.center;
            dst.size = box.size;
        }
        else if (src is SphereCollider sph)
        {
            var dst = go.AddComponent<SphereCollider>();
            CopyCommon(dst, src);
            dst.center = sph.center;
            dst.radius = sph.radius;
        }
        else if (src is CapsuleCollider cap)
        {
            var dst = go.AddComponent<CapsuleCollider>();
            CopyCommon(dst, src);
            dst.center = cap.center;
            dst.radius = cap.radius;
            dst.height = cap.height;
            dst.direction = cap.direction;
        }
        else if (src is MeshCollider mesh)
        {
            var dst = go.AddComponent<MeshCollider>();
            CopyCommon(dst, src);
            dst.sharedMesh = mesh.sharedMesh;
            dst.convex = mesh.convex;
            dst.inflateMesh = mesh.inflateMesh;
            dst.skinWidth = mesh.skinWidth;
        }
        else
        {
            // 未対応コライダー（TerrainCollider等）はそのまま子に移せないため、警告のみ
            Debug.LogWarning($"[ColliderCombiner] Unsupported collider type: {src.GetType().Name} on {src.name}");
        }

        return go;
    }

    private void CopyCommon(Collider dst, Collider src)
    {
        dst.isTrigger = src.isTrigger;
        if (copyPhysicMaterial)
        {
            dst.sharedMaterial = src.sharedMaterial;
        }
        dst.enabled = src.enabled; // 有効状態も踏襲（後で元を無効化/削除するかは設定次第）
    }

    /// <summary>
    /// CombinedColliders 配下の BoxCollider を簡易マージ（軸整列&同回転・同スケール想定）
    /// </summary>
    private void TryMergeBoxesUnder(Transform root, float grid)
    {
        var boxes = root.GetComponentsInChildren<BoxCollider>();
        if (boxes.Length <= 1) return;

        // グリッドスナップして Bounds を比較、接しているものを貪欲に結合
        // 制約: 同一回転（親と同じ回転）& 等方スケール想定。複雑な回転は対象外。
        List<BoxCollider> list = new(boxes);
        bool mergedAny = true;

        while (mergedAny)
        {
            mergedAny = false;

            for (int i = 0; i < list.Count && !mergedAny; i++)
            {
                var a = list[i];
                if (a == null) continue;

                // ワールド AABB をグリッド化
                var ab = WorldAABB(a);
                SnapBounds(ref ab, grid);

                for (int j = i + 1; j < list.Count && !mergedAny; j++)
                {
                    var b = list[j];
                    if (b == null) continue;

                    // 回転が一致している前提（親直下に作っているので一致しているはず）
                    var bb = WorldAABB(b);
                    SnapBounds(ref bb, grid);

                    if (AreMergeable(ab, bb))
                    {
                        // 結合して新しい1個に
                        Bounds mb = MergeBounds(ab, bb);

                        // a のTransformに合わせて center/size を再計算（ローカル）
                        // ここでは root 直下で回転なし・等方スケールを想定して簡易化
                        var parent = a.transform.parent;
                        Vector3 localCenter = parent.InverseTransformPoint(mb.center);
                        Vector3 localSize = mb.size; // 等方スケール前提

                        a.center = localCenter - a.transform.localPosition; // 生成時 center はローカル座標系
                        a.size = localSize;

                        // b を削除
#if UNITY_EDITOR
                        if (!Application.isPlaying) Undo.DestroyObjectImmediate(b.gameObject);
                        else Destroy(b.gameObject);
#else
                        Destroy(b.gameObject);
#endif
                        list.RemoveAt(j);
                        mergedAny = true;
                    }
                }
            }
        }
    }

    private static Bounds WorldAABB(BoxCollider bc)
    {
        var t = bc.transform;
        // ローカルBoxをワールドAABBに変換（近似：RendererのBounds風に）
        var size = Vector3.Scale(bc.size, t.lossyScale);
        var center = t.TransformPoint(bc.center);
        return new Bounds(center, Abs(size));
    }

    private static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    private static void SnapBounds(ref Bounds b, float grid)
    {
        if (grid <= 0f) return;
        Vector3 min = b.min, max = b.max;
        min = new Vector3(Snap(min.x, grid), Snap(min.y, grid), Snap(min.z, grid));
        max = new Vector3(Snap(max.x, grid), Snap(max.y, grid), Snap(max.z, grid));
        b.SetMinMax(min, max);
    }

    private static float Snap(float v, float g) => Mathf.Round(v / g) * g;

    private static bool AreMergeable(Bounds a, Bounds b)
    {
        // 2つのAABBが “1軸でのみ接し” かつ 他2軸は完全一致 のとき結合可能とする
        bool xTouch = Mathf.Approximately(a.max.x, b.min.x) || Mathf.Approximately(b.max.x, a.min.x);
        bool yTouch = Mathf.Approximately(a.max.y, b.min.y) || Mathf.Approximately(b.max.y, a.min.y);
        bool zTouch = Mathf.Approximately(a.max.z, b.min.z) || Mathf.Approximately(b.max.z, a.min.z);

        bool yEqual = Mathf.Approximately(a.min.y, b.min.y) && Mathf.Approximately(a.max.y, b.max.y);
        bool zEqual = Mathf.Approximately(a.min.z, b.min.z) && Mathf.Approximately(a.max.z, b.max.z);
        bool xEqual = Mathf.Approximately(a.min.x, b.min.x) && Mathf.Approximately(a.max.x, b.max.x);

        // X方向で接して Y/Z 完全一致 など
        if (xTouch && yEqual && zEqual) return true;
        if (yTouch && xEqual && zEqual) return true;
        if (zTouch && xEqual && yEqual) return true;

        return false;
    }

    private static Bounds MergeBounds(Bounds a, Bounds b)
    {
        Bounds c = a;
        c.Encapsulate(b.min);
        c.Encapsulate(b.max);
        return c;
    }
}
