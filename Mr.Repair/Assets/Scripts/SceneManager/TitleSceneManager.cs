using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainSceneName = "Main";

    private void Start()
    {
        // スタートボタンでMainシーンを非同期ロード
        startButton.onClick.AddListener(() => StartCoroutine(LoadMainScene()));

        // 終了ボタンでアプリ終了
        quitButton.onClick.AddListener(QuitGame);
    }

    private System.Collections.IEnumerator LoadMainScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(mainSceneName);
        async.allowSceneActivation = false;

        // 90%以上読み込んだらシーン切り替え
        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
