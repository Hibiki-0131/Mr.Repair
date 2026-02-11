using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [Header("Sound System References")]
    [SerializeField] private SoundTrigger soundTrigger;
    [SerializeField] private float footstepInterval = 0.2f;

    private PlayerMovement movement;
    private PlayerPresentationController presentation;

    private float footstepTimer;
    private bool lastPartsMode = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        presentation = GetComponent<PlayerPresentationController>();
    }

    private void Start()
    {
        if (soundTrigger == null)
        {
            soundTrigger = FindObjectOfType<SoundTrigger>();
            if (soundTrigger == null)
                Debug.LogWarning("SoundTrigger Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB");
        }
    }

    private void Update()
    {
        if (movement == null || soundTrigger == null)
            return;

        //----------------------------------
        // Åö ââèoíÜÇÕSEí‚é~
        //----------------------------------
        if (presentation != null && presentation.IsLocked)
        {
            footstepTimer = 0f;
            return;
        }

        HandleFootstepSound();
        HandlePartsTransformSound();
    }

    private void HandleFootstepSound()
    {
        bool isWalking = movement.IsMoving && !movement.IsPartsMode;
        bool isPartsWalking = movement.IsMoving && movement.IsPartsMode;

        if (isWalking)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                soundTrigger.PlayByKey("walk", transform.position);
                footstepTimer = footstepInterval;
            }
        }
        else if (isPartsWalking)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                soundTrigger.PlayByKey("parts_walk", transform.position);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void HandlePartsTransformSound()
    {
        bool current = movement.IsPartsMode;

        if (current != lastPartsMode)
        {
            if (current)
                soundTrigger.PlayByKey("parts_transform", transform.position);
            else
                soundTrigger.PlayByKey("parts_restore", transform.position);
        }

        lastPartsMode = current;
    }

    public void PlaySwitchSound(Vector3 position)
    {
        if (soundTrigger == null) return;
        soundTrigger.PlayByKey("switch_press", position);
    }
}
