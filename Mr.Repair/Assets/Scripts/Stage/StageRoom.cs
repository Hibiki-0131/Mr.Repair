using UnityEngine;

public class StageRoom : MonoBehaviour, IStageRoom
{
    private ResettableStageController resetController;

    private void Awake()
    {
        resetController = GetComponent<ResettableStageController>();
    }

    public void ResetRoom()
    {
        // š RuntimeManager‚É“ˆê
        StageRuntimeManager.Instance.ResetStage();
    }
}
