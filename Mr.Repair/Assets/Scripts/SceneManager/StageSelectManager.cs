using UnityEngine;
using UnityEngine.UI; // ← これが必要です！

public class StageSelectManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private StageSelectButton buttonPrefab; // ボタンのプレハブ
    [SerializeField] private Transform container;           // ScrollViewのContentなど

    private void Start()
    {
        GenerateButtons();
    }

    private void GenerateButtons()
    {
        // 親要素の下にある古いボタンを掃除
        foreach (Transform child in container) Destroy(child.gameObject);

        int totalStages = StageManager.Instance.GetTotalStageCount();
        int reachedIndex = StageManager.Instance.ReachedStageIndex;

        for (int i = 0; i < totalStages; i++)
        {
            StageSelectButton btn = Instantiate(buttonPrefab, container);

            bool isUnlocked = i <= reachedIndex;
            string rawName = StageManager.Instance.GetStageNameAt(i);

            // シーン名整形
            string displayName = rawName.Replace("Stage", "").Replace("_", "-");

            btn.Setup(i, isUnlocked, displayName);
        }

        // --- 追加した「FirstSelected」の処理 ---
        if (container.childCount > 0)
        {
            // GetComponent<Button>() を使うために using UnityEngine.UI; が必要
            var firstButton = container.GetChild(0).GetComponent<Button>();
            if (firstButton != null)
            {
                firstButton.Select();
            }
        }
    }

    // ステージ選択シーンの「Back」ボタンに紐付ける
    public void OnClickBackButton()
    {
        SceneController.Instance.LoadSceneAsync("Title");
    }
}