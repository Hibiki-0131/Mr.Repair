using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadZoneHandler : MonoBehaviour
{
    [SerializeField] private string gameOverSceneName = "GameOver";

    private void OnTriggerEnter(Collider other)
    {
        // DeadZoneタグに触れたらゲームオーバーシーンへ
        if (other.CompareTag("DeathZone"))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}
