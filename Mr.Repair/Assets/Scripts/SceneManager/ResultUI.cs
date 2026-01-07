using UnityEngine;

public class ResultUI : MonoBehaviour
{
    public void OnRetry()
    {
        StageManager.Instance.RetryCurrentStage();
    }

    public void OnReturnToTitle()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Title);
        SceneController.Instance.LoadSceneAsync("Title");
    }
}

