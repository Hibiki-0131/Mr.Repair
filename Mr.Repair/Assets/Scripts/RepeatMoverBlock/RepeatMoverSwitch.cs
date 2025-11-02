using UnityEngine;

public class RepeatMoverSwitch : MonoBehaviour
{
    [SerializeField] private string targetID;
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.CompareTag("Player"))
        {
            isPressed = true;
            RepeatMoverManager.Instance?.ToggleBlock(targetID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPressed && other.CompareTag("Player"))
            isPressed = false;
    }
}
