using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PersistentAudioSource : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private SoundCategory category = SoundCategory.BGM;
    private int lastUpdateCount = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (SoundManager.Instance == null) return;

        // SoundManagerの更新カウントが変わっていたら、音量を更新する
        if (lastUpdateCount != SoundManager.Instance.VolumeUpdateCount)
        {
            lastUpdateCount = SoundManager.Instance.VolumeUpdateCount;
            UpdateVolume();
        }
    }

    private void UpdateVolume()
    {
        float newVolume = SoundManager.Instance.GetVolume(category);
        audioSource.volume = newVolume;
        Debug.Log($"{gameObject.name} の音量を {newVolume} に更新しました！");
    }
}