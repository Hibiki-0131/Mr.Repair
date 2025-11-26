using UnityEngine;

public class RepeatMoverSwitch : MonoBehaviour
{
    [SerializeField] private string targetID;
    private bool isPressed = false;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

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

            // スイッチの押下動作
            RepeatMoverManager.Instance?.ToggleBlock(targetID);

            // コライダーを無効化して再押下を防止
            if (col != null)
                col.enabled = false;

            // 必要なら押されたアニメーションや音も再生できる
            // e.g. GetComponent<Animator>()?.SetTrigger("Pressed");
        }
    }
}
