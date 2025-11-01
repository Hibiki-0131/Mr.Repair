using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // ゲームオーバー画面へ遷移
        StartCoroutine(GoToGameOver());
    }

    private System.Collections.IEnumerator GoToGameOver()
    {
        // 少し待ってからゲームオーバーへ
        yield return new WaitForSeconds(2f);
        SceneController.Instance.LoadSceneAsync("GameOver");
    }
}
