using UnityEngine;

public class PlayerClearHandler : MonoBehaviour
{
    private bool isCleared = false;

    public void Clear()
    {
        if (isCleared) return;
        isCleared = true;

        StartCoroutine(GoToNextStage());
    }

    private System.Collections.IEnumerator GoToNextStage()
    {
        yield return new WaitForSeconds(0f);

        // ★ここを変更：次のステージへ進む
        StageManager.Instance.ClearStage();
    }
}
