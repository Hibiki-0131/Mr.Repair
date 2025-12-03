using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("ロードしたい順番でシーン名を並べる")]
    [SerializeField] private List<string> stageOrder = new List<string>();

    private int currentStageIndex = 0;

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

    public void StartFirstStage()
    {
        currentStageIndex = 0;
        LoadCurrentStage();
    }

    public void ClearStage()
    {
        currentStageIndex++;

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

    public string GetCurrentStageName()
    {
        if (currentStageIndex < 0 || currentStageIndex >= stageOrder.Count)
            return "Unknown";

        return stageOrder[currentStageIndex];
    }
}
