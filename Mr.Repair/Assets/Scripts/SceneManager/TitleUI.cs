using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void OnStartButton()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        StageManager.Instance.StartFirstStage();   // ← 追加！
    }

    public void OnStageSelectButton()
    {
        // "StageSelectScene" の部分は、作成したシーン名に合わせてください
        SceneController.Instance.LoadSceneAsync("StageSelectScene");
    }

    public void OnExitButton() => Application.Quit();
}
