using UnityEngine;

public class SlideSwitch : MonoBehaviour
{
    [SerializeField] private string targetID;
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.CompareTag("Player"))
        {
            isPressed = true;
            SlideManager.Instance?.ActivateBlock(targetID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPressed && other.CompareTag("Player"))
        {
            isPressed = false; // Ä‚Ñ‰Ÿ‚¹‚é‚æ‚¤‚É‚·‚é
        }
    }
}
