using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void OnStartButton()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Playing);
        StageManager.Instance.StartFirstStage();   // © ’Ç‰ÁI
    }

    public void OnExitButton() => Application.Quit();
}
