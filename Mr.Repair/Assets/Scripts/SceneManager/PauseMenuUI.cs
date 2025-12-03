using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

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
    /// š Reset ƒ{ƒ^ƒ“‚©‚çŒÄ‚Î‚ê‚é
    /// </summary>
    public void Reset()
    {
        // Pause ‰ğœ‚µ‚Ä‚©‚ç Reset ‚·‚é‚ÆˆÀ‘S
        GameStateManager.Instance.ResumeGame();
        pauseUI.SetActive(false);

        ResettableStageController.Instance.ResetStage();
    }
}
