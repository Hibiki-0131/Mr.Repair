using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/CrankHandleEffect")]
public class CrankHandleEffect : ItemEffectBase
{
    [Header("アクティブ化するオブジェクト名")]
    public string targetChildName = "HiddenHandle";  // 任意で変更可

    public override void ExecuteEffect(GameObject target)
    {
        Transform handle = FindDeepChild(target.transform, targetChildName);

        if (handle != null)
        {
            handle.gameObject.SetActive(true);
            Debug.Log($"{targetChildName} をアクティブにしました。");
        }
        else
        {
            Debug.LogWarning($"{targetChildName} が {target.name} の子に見つかりませんでした。");
        }
    }

    // 階層の深い子まで探すヘルパー関数
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
