using UnityEngine;

public class SlideSwitch : MonoBehaviour
{
    [SerializeField] private string targetID; // ‘Î‰‚·‚éƒuƒƒbƒNID
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;
            SlideManager.Instance?.ActivateBlock(targetID);
        }
    }
}
