using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateManager.Instance.IsPaused)
                Resume();
            else
                Pause();
        }
    }

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
}
