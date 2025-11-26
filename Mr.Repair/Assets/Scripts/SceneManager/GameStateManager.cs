using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        Title,
        Playing,
        Paused,
        GameOver,
        GameClear
    }

    public GameState CurrentState { get; private set; } = GameState.Title;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }

    public bool IsPaused => CurrentState == GameState.Paused;

    public void PauseGame()
    {
        if (IsPaused) return;
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }
}
