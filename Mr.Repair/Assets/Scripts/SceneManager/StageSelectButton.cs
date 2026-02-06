using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Image previewImage;      // ステージ画像用
    [SerializeField] private GameObject lockOverlay;  // 未開放時の暗いパネル
    [SerializeField] private Button button;

    private int targetIndex;

    public void Setup(int index, bool isUnlocked, string displayName)
    {
        targetIndex = index;
        if (button == null) button = GetComponent<Button>();

        // 未クリアならテキストを「???」にする
        if (stageText != null)
        {
            stageText.text = isUnlocked ? displayName : "???";
        }

        // 未クリアなら画像をグレーにする（簡易実装）
        if (previewImage != null)
        {
            previewImage.color = isUnlocked ? Color.white : Color.gray;
        }

        button.interactable = isUnlocked;
        if (lockOverlay != null) lockOverlay.SetActive(!isUnlocked);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        Debug.Log($"Stage {targetIndex} Selected");
        StageManager.Instance.SelectStage(targetIndex);
    }
}