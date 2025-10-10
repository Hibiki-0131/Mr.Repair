using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/CrankHandleEffect")]
public class CrankHandleEffect : ItemEffectBase
{
    [Header("アクティブ化するオブジェクト名")]
    public string targetChildName = "HiddenHandle";

    public override void ExecuteEffect(GameObject target)
    {
        Transform handle = FindDeepChild(target.transform, targetChildName);
        if (handle != null)
        {
            handle.gameObject.SetActive(true);
            Debug.Log($"{targetChildName} をアクティブにしました。");
            // EnableInteraction は削除
        }
        else
        {
            Debug.LogWarning($"{targetChildName} が {target.name} の子に見つかりませんでした。");
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
