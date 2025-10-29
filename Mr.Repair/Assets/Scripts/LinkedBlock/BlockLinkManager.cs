using UnityEngine;
using System.Collections.Generic;

public class BlockLinkManager : MonoBehaviour
{
    public static BlockLinkManager Instance { get; private set; }

    private Dictionary<string, PowerBlock> powerBlocks = new();
    private Dictionary<string, List<FollowerBlock>> followers = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPowerBlock(PowerBlock block)
    {
        if (!powerBlocks.ContainsKey(block.GroupID))
            powerBlocks[block.GroupID] = block;
    }

    public void RegisterFollower(FollowerBlock follower)
    {
        string id = follower.GroupID;
        if (!followers.ContainsKey(id))
            followers[id] = new List<FollowerBlock>();
        followers[id].Add(follower);
    }

    public void SyncFollowerMovement(string id, Vector3 delta)
    {
        if (followers.TryGetValue(id, out var list))
        {
            foreach (var f in list)
            {
                if (f != null)
                    f.ApplyMovement(delta);
            }
        }
    }
}
