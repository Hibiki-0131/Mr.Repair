using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void OnStartButton()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        SceneController.Instance.LoadSceneAsync("Main");
    }

    public void OnExitButton() => Application.Quit();
}
