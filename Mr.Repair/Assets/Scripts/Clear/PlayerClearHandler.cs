using UnityEngine;

public class PlayerClearHandler : MonoBehaviour
{
    private bool isCleared = false;

    public void Clear()
    {
        Debug.Log("<color=green>[ClearHandler] Clear() called!</color>");
        if (isCleared) return;
        isCleared = true;

        StartCoroutine(GoToNextStage());
    }

    private System.Collections.IEnumerator GoToNextStage()
    {
        Debug.Log("<color=green>[ClearHandler] GoToNextStage Coroutine Started</color>");
        yield return new WaitForSeconds(0.5f); // è≠Çµó]óTÇéùÇΩÇπÇÈ

        if (StageManager.Instance != null)
        {
            Debug.Log("<color=green>[ClearHandler] Calling StageManager.ClearStage()</color>");
            StageManager.Instance.ClearStage();
        }
        else
        {
            Debug.LogError("<color=red>[ClearHandler] StageManager.Instance is NULL!</color>");
        }
    }
}
