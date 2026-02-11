using UnityEngine;

public class FallResetTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (StageRuntimeManager.Instance == null)
        {
            Debug.LogError("[FallResetTrigger] StageRuntimeManager not found");
            return;
        }

        StageRuntimeManager.EnsureExists().ResetStage();
    }
}
