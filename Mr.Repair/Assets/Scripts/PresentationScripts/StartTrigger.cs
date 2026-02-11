using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StartTrigger : MonoBehaviour
{
    [SerializeField] private float lockDuration = 2f;
    [SerializeField] private SoundTrigger soundTrigger; // š’Ç‰Á

    private bool triggered;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Start()
    {
        // š SoundTrigger ©“®æ“¾
        if (soundTrigger == null)
            soundTrigger = FindObjectOfType<SoundTrigger>();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var presentation = player.GetComponent<PlayerPresentationController>();
        if (presentation == null) return;

        //-----------------------------------
        // š StartSEÄ¶
        //-----------------------------------
        soundTrigger?.PlayByKey("StartSE", player.transform.position);

        presentation.LockControl(lockDuration);
        triggered = true;
    }
}
