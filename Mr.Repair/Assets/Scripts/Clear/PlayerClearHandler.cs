using UnityEngine;

public class PlayerClearHandler : MonoBehaviour
{
    private bool isCleared = false;

    public void Clear()
    {
        if (isCleared) return;
        isCleared = true;

        // ゲームオーバー画面へ遷移
        StartCoroutine(GoToGameClear());
    }

    private System.Collections.IEnumerator GoToGameClear()
    {
        // 少し待ってからゲームオーバーへ
        yield return new WaitForSeconds(2f);
        SceneController.Instance.LoadSceneAsync("GameClear");
    }
}
