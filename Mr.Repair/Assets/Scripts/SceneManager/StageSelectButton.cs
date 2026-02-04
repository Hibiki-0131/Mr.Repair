using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject lockOverlay; // 未開放時に表示する暗いパネルなど
    [SerializeField] private Button button;

    private int targetIndex;

    public void Setup(int index, bool isUnlocked, string displayName)
    {
        targetIndex = index;
        // buttonが未設定なら、ここで取得を試みる
        if (button == null) button = GetComponent<Button>();

        if (stageText != null) stageText.text = displayName;

        button.interactable = isUnlocked;
        if (lockOverlay != null) lockOverlay.SetActive(!isUnlocked);

        // 二重登録を防ぎつつ、確実にクリックイベントを登録
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            Debug.Log($"<color=orange>Internal Click: {targetIndex}</color>");
            OnClicked();
        });
    }

    private void OnClicked()
    {
        Debug.Log($"<color=orange>Button Clicked: Index {targetIndex}</color>");
        StageManager.Instance.SelectStage(targetIndex);
    }
}