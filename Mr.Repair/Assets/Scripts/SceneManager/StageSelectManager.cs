using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private StageSelectButton buttonPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Scroll設定")]
    [SerializeField] private bool smoothScroll = true;
    [SerializeField] private float scrollSpeed = 10f;

    private bool generated = false;

    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        Debug.Log("<color=cyan>[StageSelect] Initialize</color>");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GenerateButtons();
    }

    // =========================================================
    // Update（選択追従）
    // =========================================================

    private void Update()
    {
        FollowSelectedButton();
    }

    // =========================================================
    // ボタン生成
    // =========================================================

    private void GenerateButtons()
    {
        if (buttonPrefab == null || container == null)
        {
            Debug.LogError("[StageSelect] Prefab or Container missing!");
            return;
        }

        foreach (Transform child in container)
            Destroy(child.gameObject);

        int totalStages = StageManager.Instance.GetTotalStageCount();
        int reachedIndex = StageManager.Instance.ReachedStageIndex;

        Debug.Log($"[StageSelect] Generate Buttons : {totalStages}");

        for (int i = 0; i < totalStages; i++)
        {
            StageSelectButton btn = Instantiate(buttonPrefab, container);

            bool isUnlocked = i <= reachedIndex;

            string rawName = StageManager.Instance.GetStageNameAt(i);
            string displayName = rawName.Replace("Stage", "").Replace("_", "-");

            btn.Setup(i, isUnlocked, displayName);
        }

        generated = true;

        // ★最初のボタンを自動選択（超重要）
        SelectFirstButton();
    }

    // =========================================================
    // 初期選択
    // =========================================================

    private void SelectFirstButton()
    {
        if (container.childCount == 0) return;

        Button first = container.GetChild(0).GetComponent<Button>();
        if (first != null)
        {
            EventSystem.current.SetSelectedGameObject(first.gameObject);
            Debug.Log("<color=cyan>[StageSelect] First Selected → Stage 0</color>");
        }
    }

    // =========================================================
    // 選択中ボタンを自動追尾スクロール（核心処理）
    // =========================================================

    private void FollowSelectedButton()
    {
        if (!generated || scrollRect == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        RectTransform target = selected.GetComponent<RectTransform>();
        if (target == null) return;

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        if (contentWidth <= viewportWidth / 2) return;

        // ボタン位置取得
        float targetPosX = Mathf.Abs(target.anchoredPosition.x);

        float centerOffset = viewportWidth * 0.5f;

        float normalized =
            (targetPosX - centerOffset) / (contentWidth - viewportWidth);

        normalized = Mathf.Clamp01(normalized);

        // スムーズ or 即時
        if (smoothScroll)
        {
            scrollRect.horizontalNormalizedPosition =
                Mathf.Lerp(scrollRect.horizontalNormalizedPosition, normalized,
                           scrollSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = normalized;
        }
    }

    // =========================================================
    // 戻るボタン
    // =========================================================

    public void OnClickBackButton()
    {
        SceneController.Instance.LoadSceneAsync("Title");
    }
}
