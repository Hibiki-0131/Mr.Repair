using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerFootstepController : MonoBehaviour
{
    [SerializeField] private float stepInterval = 0.5f;   // 歩行音の間隔
    [SerializeField] private SoundDatabase database;      // walk音の登録があるSoundDatabase

    private float stepTimer = 0f;
    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (movement == null || SoundPlayer.Instance == null) return;

        // 移動中かつ部品化していないときのみ歩行音を鳴らす
        bool isWalking = movement.IsMoving && !movement.IsPartsMode;

        if (isWalking)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                var sound = database.GetSound("walk");
                if (sound != null)
                    SoundPlayer.Instance.PlaySound(sound.clip, sound.category, transform.position);

                stepTimer = stepInterval; // タイマーをリセット
            }
        }
        else
        {
            stepTimer = 0f; // 停止中はリセット
        }
    }
}
