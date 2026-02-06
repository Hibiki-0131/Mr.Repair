using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("ロードしたい順番でシーン名を並べる")]
    [SerializeField] private List<string> stageOrder = new List<string>();

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
        {
            Debug.LogError("StageManager にステージが1つも登録されていません！");
        }
    }

    // タイトルやリザルトから呼ばれる既存メソッド
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

    public string GetCurrentStageName()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stageOrder.Count) return "Unknown";
        return stageOrder[currentStageIndex];
    }

    // ステージ選択用メソッド
    public void SelectStage(int index)
    {
        if (index < 0 || index >= stageOrder.Count) return;
        currentStageIndex = index;
        LoadCurrentStage();
    }

    public int GetTotalStageCount() => stageOrder.Count;
    public string GetStageNameAt(int index) => (index >= 0 && index < stageOrder.Count) ? stageOrder[index] : "";

    public void ClearStage()
    {
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
        if (currentStageIndex < 0 || currentStageIndex >= stageOrder.Count) return;
        string sceneName = stageOrder[currentStageIndex];
        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        SceneController.Instance.LoadSceneAsync(sceneName);
    }
}