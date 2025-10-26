using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private int poolSize = 10;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        // プールの初期生成
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(robotPrefab);
            obj.SetActive(false); // 初期状態は非アクティブ
            pool.Add(obj);
        }
    }

    // プールから使えるオブジェクトを取得
    public GameObject GetFromPool()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // プールが足りなければ新規生成して追加
        GameObject newObj = Instantiate(robotPrefab);
        pool.Add(newObj);
        return newObj;
    }

    // 使用後に戻す
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
