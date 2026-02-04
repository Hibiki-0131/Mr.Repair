using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("ロードしたい順番でシーン名を並べる")]
    [SerializeField] private List<string> stageOrder = new List<string>();

    private int currentStageIndex = 0;
    private const string ClearKey = "ReachedStageIndex";

    // クリア済みの最大ステージインデックスをPlayerPrefsで管理
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
        {
            Debug.LogError("StageManager にステージが1つも登録されていません！");
        }
    }

    // =========================================================
    // 既存の機能 (TitleUI / ResultUI 等から呼ばれる)
    // =========================================================

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

    public void RetryCurrentStage()
    {
        LoadCurrentStage();
    }

    public string GetCurrentStageName()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stageOrder.Count)
            return "Unknown";

        return stageOrder[currentStageIndex];
    }

    // =========================================================
    // 新機能 (ステージ選択画面用)
    // =========================================================

    public void SelectStage(int index)
    {
        if (index < 0 || index >= stageOrder.Count) return;
        currentStageIndex = index;
        LoadCurrentStage();
    }

    public int GetTotalStageCount() => stageOrder.Count;
    public string GetStageNameAt(int index) => (index >= 0 && index < stageOrder.Count) ? stageOrder[index] : "";

    // =========================================================
    // 共通ロジック
    // =========================================================

    public void ClearStage()
    {
        // 進捗を更新
        int nextIndex = currentStageIndex + 1;
        if (nextIndex > ReachedStageIndex)
        {
            ReachedStageIndex = nextIndex;
        }

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
        if (currentStageIndex < 0 || currentStageIndex >= stageOrder.Count)
        {
            Debug.LogError("Stage index が範囲外です");
            return;
        }

        string sceneName = stageOrder[currentStageIndex];
        Debug.Log("Load Stage: " + sceneName);

        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        SceneController.Instance.LoadSceneAsync(sceneName);
    }
}