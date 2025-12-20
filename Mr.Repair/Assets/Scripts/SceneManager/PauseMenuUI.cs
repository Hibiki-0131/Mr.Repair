using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    [Header("Stage Reset")]
    [SerializeField] private ResettableStageController resetController;

    public void Pause()
    {
        pauseUI.SetActive(true);
        GameStateManager.Instance.PauseGame();
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        GameStateManager.Instance.ResumeGame();
    }

    public void ReturnToTitle()
    {
        GameStateManager.Instance.SetState(GameStateManager.GameState.Title);
        SceneController.Instance.LoadSceneAsync("Title");
        GameStateManager.Instance.ResumeGame();
    }

    /// <summary>
    /// Reset ƒ{ƒ^ƒ“‚©‚çŒÄ‚Î‚ê‚é
    /// </summary>
    public void Reset()
    {
        GameStateManager.Instance.ResumeGame();
        pauseUI.SetActive(false);

        if (resetController != null)
        {
            resetController.ResetStage();
        }
        else
        {
            Debug.LogError(
                "[PauseMenuUI] ResettableStageController ‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ",
                this
            );
        }
    }
}
