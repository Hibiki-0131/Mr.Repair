using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [Header("Sound System References")]
    [SerializeField] private SoundTrigger soundTrigger; // ← Inspector未設定でもOK
    [SerializeField] private float footstepInterval = 0.2f;

    private PlayerMovement movement;
    private float footstepTimer;
    private bool lastPartsMode = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        // SoundTrigger が未設定なら自動検索（DontDestroyOnLoad対応）
        if (soundTrigger == null)
        {
            soundTrigger = FindObjectOfType<SoundTrigger>();
            if (soundTrigger == null)
                Debug.LogWarning("SoundTrigger が見つかりません。SoundManager がロードされているか確認してください。");
        }
    }

    private void Update()
    {
        if (movement == null || soundTrigger == null) return;

        HandleFootstepSound();
        HandlePartsTransformSound();
    }

    private void HandleFootstepSound()
    {
        bool isWalking = movement.IsMoving && !movement.IsPartsMode;

        if (isWalking)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                soundTrigger.PlayByKey("walk", transform.position);
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
}
