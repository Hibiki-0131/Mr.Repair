using UnityEngine;

public class ResultUI : MonoBehaviour
{
    /// <summary>
    /// 「リトライ」ボタン
    /// </summary>
    public void OnRetry()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        SceneController.Instance.LoadSceneAsync("1_1");
    }

    /// <summary>
    /// 「タイトルに戻る」ボタン
    /// </summary>
    public void OnReturnToTitle()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Title);
        SceneController.Instance.LoadSceneAsync("Title");
    }
}
