using UnityEngine;
using System.Collections.Generic;

public class BlockLinkManager : MonoBehaviour
{
    public static BlockLinkManager Instance { get; private set; }
    private Dictionary<string, PowerBlock> powerBlocks = new();
    private Dictionary<string, List<FollowerBlock>> followers = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterPowerBlock(PowerBlock block) => powerBlocks[block.GroupID] = block;
    public void RegisterFollower(FollowerBlock follower)
    {
        if (!followers.ContainsKey(follower.GroupID)) followers[follower.GroupID] = new List<FollowerBlock>();
        followers[follower.GroupID].Add(follower);
    }

    public void SyncFollowerMovement(string id, Vector3 delta)
    {
        if (followers.TryGetValue(id, out var list))
        {
            float minPossibleDistanceRatio = 1f;

            foreach (var f in list)
            {
                if (f == null) continue;
                // Followerがどれくらい動けるかチェック
                float ratio = f.CheckMovementRatio(delta);
                if (ratio < minPossibleDistanceRatio) minPossibleDistanceRatio = ratio;
            }

            // 全員が動ける最小範囲に合わせる
            Vector3 finalDelta = delta * minPossibleDistanceRatio;
            foreach (var f in list)
            {
                if (f != null) f.ApplyMovement(finalDelta);
            }

            // PowerBlockの位置を補正（Followerが詰まったらPowerも戻す）
            if (minPossibleDistanceRatio < 1f && powerBlocks.TryGetValue(id, out var p))
            {
                p.ForceReposition(finalDelta);
            }
        }
    }
}