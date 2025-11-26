using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

#if UNITY_EDITOR
    // Editor でだけ SceneAsset を使う
    public UnityEditor.SceneAsset[] stageSceneAssets;
#endif

    [SerializeField]
    private string[] stageNames;  // 実際にロードするシーン名

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

#if UNITY_EDITOR
        // エディタ上でシーン名を自動更新
        UpdateSceneNamesFromAssets();
#endif
    }

#if UNITY_EDITOR
    private void UpdateSceneNamesFromAssets()
    {
        if (stageSceneAssets == null) return;

        stageNames = new string[stageSceneAssets.Length];

        for (int i = 0; i < stageSceneAssets.Length; i++)
        {
            if (stageSceneAssets[i] != null)
                stageNames[i] = stageSceneAssets[i].name;
        }
    }
#endif

    public void StartFirstStage()
    {
        currentStageIndex = 0;
        LoadCurrentStage();
    }

    public void ClearStage()
    {
        currentStageIndex++;

        if (currentStageIndex >= stageNames.Length)
        {
            // 全ステージクリア
            GameStateManager.Instance.SetState(GameStateManager.GameState.GameClear);
            SceneController.Instance.LoadSceneAsync("Result");
            return;
        }

        LoadCurrentStage();
    }

    private void LoadCurrentStage()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        SceneController.Instance.LoadSceneAsync(stageNames[currentStageIndex]);
    }

    public string GetCurrentStageName()
    {
        return stageNames[currentStageIndex];
    }
}
