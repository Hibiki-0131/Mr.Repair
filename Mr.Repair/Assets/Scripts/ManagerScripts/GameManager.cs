using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isGameOver = false;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =====================
    // ▼ Game Over 管理
    // =====================
    public void SetGameOver(bool value)
    {
        isGameOver = value;
        Debug.Log($"GameManager: ゲームオーバー状態 = {isGameOver}");

        if (isGameOver && SceneController.Instance != null)
        {
            SceneController.Instance.LoadGameOver();
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    // =====================
    // ▼ Pause 管理
    // =====================
    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
