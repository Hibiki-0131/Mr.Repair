using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("ロード順（シーン名）")]
    [SerializeField] private List<string> stageOrder = new List<string>();

    [Header("各ステージのプレビュー画像（同じ順番で並べる）")]
    [SerializeField] private List<Sprite> stageSprites = new List<Sprite>(); // ★追加

    private int currentStageIndex = 0;
    private const string ClearKey = "ReachedStageIndex";

    public int ReachedStageIndex
    {
        get => PlayerPrefs.GetInt(ClearKey, 0);
        private set
        {
            PlayerPrefs.SetInt(ClearKey, value);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (stageOrder.Count == 0)
            Debug.LogError("StageManager にステージが登録されていません");

        // ★安全チェック（地味に超重要）
        if (stageSprites.Count != stageOrder.Count)
            Debug.LogWarning("stageOrder と stageSprites の数が一致していません！");
    }

    // =========================
    // 追加：Sprite取得API（超重要）
    // =========================
    public Sprite GetStageSprite(int index)
    {
        if (index < 0 || index >= stageSprites.Count)
            return null;

        return stageSprites[index];
    }

    // =========================
    // 既存機能
    // =========================

    public void StartFirstStage()
    {
        currentStageIndex = 0;
        LoadCurrentStage();
    }

    public void RetryFromBeginning()
    {
        currentStageIndex = 0;
        LoadCurrentStage();
    }

    public void RetryCurrentStage() => LoadCurrentStage();

    public int GetTotalStageCount() => stageOrder.Count;

    public string GetStageNameAt(int index)
    {
        return (index >= 0 && index < stageOrder.Count)
            ? stageOrder[index]
            : "";
    }

    public void SelectStage(int index)
    {
        if (index < 0 || index >= stageOrder.Count) return;

        currentStageIndex = index;
        LoadCurrentStage();
    }

    public void ClearStage()
    {
        int nextIndex = currentStageIndex + 1;

        if (nextIndex > ReachedStageIndex)
            ReachedStageIndex = nextIndex;

        currentStageIndex = nextIndex;

        if (currentStageIndex >= stageOrder.Count)
        {
            GameStateManager.Instance.SetState(GameStateManager.GameState.GameClear);
            SceneController.Instance.LoadSceneAsync("Result");
            return;
        }

        LoadCurrentStage();
    }

    private void LoadCurrentStage()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stageOrder.Count) return;

        string sceneName = stageOrder[currentStageIndex];

        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        SceneController.Instance.LoadSceneAsync(sceneName);
    }
}
