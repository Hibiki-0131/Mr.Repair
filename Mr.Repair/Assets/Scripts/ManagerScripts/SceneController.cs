using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Šù‘¶‚Ì Instance ‚ª‚ ‚éê‡‚Í”jŠü
        }
    }

    public void LoadTitle() => SceneManager.LoadScene("Title");
    public void LoadMain() => SceneManager.LoadScene("Station");
    public void LoadGameOver() => SceneManager.LoadScene("GameOver");
    public void LoadGameClear() => SceneManager.LoadScene("GameClear");

    public void QuitGame()
    {
        Debug.Log("QuitGame called");
        Application.Quit();
    }
}
