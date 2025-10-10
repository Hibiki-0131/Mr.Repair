using UnityEngine;
using UnityEngine.SceneManagement;  // シーン遷移用

public class GameClear : MonoBehaviour
{
    [SerializeField] private string clearSceneName = "GameClear"; // 遷移先シーン名

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに触れたら
        if (other.CompareTag("Player"))
        {
            // シーン遷移
            SceneManager.LoadScene(clearSceneName);
        }
    }
}
