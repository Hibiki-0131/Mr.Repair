using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    // Input System
    // =========================================================
    private DebugInputActions inputActions;

    private void Awake()
    {
        inputActions = new DebugInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Debug.ToggleUnlockAll.performed += OnToggleUnlockAll;
    }

    private void OnDisable()
    {
        inputActions.Debug.ToggleUnlockAll.performed -= OnToggleUnlockAll;
        inputActions.Disable();
    }

    private void OnToggleUnlockAll(InputAction.CallbackContext ctx)
    {
        StageManager.Instance.ToggleDebugUnlockAll();

        GenerateButtons();

        StartCoroutine(ReSelectNextFrame());
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GenerateButtons();
    }

    // =========================================================
    // Update
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
        if (buttonPrefab == null || container == null) return;

        foreach (Transform child in container)
            Destroy(child.gameObject);

        int totalStages = StageManager.Instance.GetTotalStageCount();

        for (int i = 0; i < totalStages; i++)
        {
            StageSelectButton btn = Instantiate(buttonPrefab, container);

            bool isUnlocked = StageManager.Instance.IsStageUnlocked(i);

            string rawName = StageManager.Instance.GetStageNameAt(i);
            string displayName = rawName.Replace("Stage", "").Replace("_", "-");

            btn.Setup(i, isUnlocked, displayName);
        }

        generated = true;
        SelectFirstButton();
    }

    // =========================================================
    // 初期選択
    // =========================================================

    private void SelectFirstButton()
    {
        if (container.childCount == 0) return;

        Button first = container.GetChild(0).GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    // =========================================================
    // スクロール追従
    // =========================================================

    private void FollowSelectedButton()
    {
        if (!generated || scrollRect == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        if (selected.transform.parent != container) return;

        RectTransform target = selected.GetComponent<RectTransform>();
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        if (content.rect.width <= viewport.rect.width) return;

        float targetPosX = target.anchoredPosition.x;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;
        float scrollRange = contentWidth - viewportWidth;

        float normalized = (targetPosX + buttonSize - (viewportWidth * 0.5f)) / scrollRange;
        normalized = Mathf.Clamp01(normalized);

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

    public void OnClickBackButton()
    {
        SceneController.Instance.LoadSceneAsync("Title");
    }

    private System.Collections.IEnumerator ReSelectNextFrame()
    {
        yield return null; // ★1フレーム待つ

        SelectFirstButton();
    }

}
