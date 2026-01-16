using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private SoundCategory category = SoundCategory.BGM;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // SoundManagerの音量変更イベントを購読
        if (SoundManager.Instance != null)
            SoundManager.Instance.OnVolumeChanged += UpdateVolume;
    }

    private void OnDisable()
    {
        // 解除を忘れずに
        if (SoundManager.Instance != null)
            SoundManager.Instance.OnVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume()
    {
        if (audioSource != null && SoundManager.Instance != null)
        {
            audioSource.volume = SoundManager.Instance.GetVolume(category);
        }
    }
}