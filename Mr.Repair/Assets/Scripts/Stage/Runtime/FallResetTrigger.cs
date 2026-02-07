using UnityEngine;

public class FallResetTrigger : MonoBehaviour
{
    [SerializeField]
    private ResettableStageController resetController;

    private void Awake()
    {
        // –¢İ’è‚È‚ç©“®æ“¾iˆÀ‘Sj
        if (resetController == null)
            resetController = FindObjectOfType<ResettableStageController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // PlayerTag ‚Ì‚İ”½‰
        if (!other.CompareTag("Player"))
            return;

        if (resetController == null)
        {
            Debug.LogError("[FallResetTrigger] ResetController not found");
            return;
        }

        resetController.ResetStage();
    }
}
