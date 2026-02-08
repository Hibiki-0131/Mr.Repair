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
    [SerializeField] private float buttonSize = 640;

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

        // 選択されているのがコンテナの子要素（ボタン）でない場合は無視
        if (selected.transform.parent != container) return;

        RectTransform target = selected.GetComponent<RectTransform>();
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // ContentがViewportより小さい場合はスクロール不要
        if (content.rect.width <= viewport.rect.width) return;

        // --- ここから計算式を修正 ---

        // 1. Content内におけるボタンのローカルX座標を取得
        float targetPosX = target.anchoredPosition.x;

        // 2. 求めたいのは「そのボタンが中央に来る時のContentのNormalizedPosition」
        // 0.0 (左端) から 1.0 (右端) の範囲で、ボタンの位置を割合で算出します。
        // 計算式： (ボタンの座標) / (Content全体の幅 - Viewportの幅) を反転させたもの
        // ※UnityのHorizontalNormalizedPositionは 0が左、1が右です。

        // アンカー設定が左端(0)の場合の計算：
        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;
        float scrollRange = contentWidth - viewportWidth;

        // ボタンのX座標（正の値）をスクロール可能範囲で割る
        // targetPosXが正の値か負の値かはRectTransformの設定によりますが、
        // 通常のHorizontal Layout Groupであれば targetPosX / scrollRange で正規化できます。
        float normalized = (targetPosX + buttonSize - (viewportWidth * 0.5f)) / scrollRange;

        // 範囲を 0~1 に制限
        normalized = Mathf.Clamp01(normalized);

        // スムーズスクロールの適用
        if (smoothScroll)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                normalized,
                scrollSpeed * Time.unscaledDeltaTime
            );
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
