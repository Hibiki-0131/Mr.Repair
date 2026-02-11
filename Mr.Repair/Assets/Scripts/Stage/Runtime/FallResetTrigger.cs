using UnityEngine;

public class FallResetTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StageRuntimeManager.EnsureExists().ResetStage();
    }
}
