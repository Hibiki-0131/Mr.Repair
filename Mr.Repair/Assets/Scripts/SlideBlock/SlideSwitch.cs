using UnityEngine;

public class SlideSwitch : MonoBehaviour
{
    [SerializeField] private string targetID;
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        var sound = other.GetComponent<PlayerSoundController>();
        if (sound != null)
        {
            sound.PlaySwitchSound(transform.position);
        }

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
            isPressed = false; // çƒÇ—âüÇπÇÈÇÊÇ§Ç…Ç∑ÇÈ
        }
    }
}
