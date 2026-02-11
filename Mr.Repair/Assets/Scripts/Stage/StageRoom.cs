using UnityEngine;

[RequireComponent(typeof(ResettableStageController))]
public class StageRoom : MonoBehaviour
{
    private void Awake()
    {
        var resetController = GetComponent<ResettableStageController>();

        StageRuntimeManager.EnsureExists()
            .RegisterRoom(resetController);
    }
}
