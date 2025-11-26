using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [SerializeField] private GameObject audioSourcePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip, SoundCategory category, Vector3 position)
    {
        if (clip == null) return;

        GameObject obj = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        AudioSource source = obj.GetComponent<AudioSource>();

        float volume = SoundManager.Instance.GetVolume(category);

        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f; // —§‘Ì‰¹‹¿
        source.minDistance = 1f;
        source.maxDistance = 50f;
        source.rolloffMode = AudioRolloffMode.Linear;

        source.Play();
        Destroy(obj, clip.length);
    }
}
