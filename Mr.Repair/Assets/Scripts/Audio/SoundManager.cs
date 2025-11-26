using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.8f;
    [Range(0f, 1f)] public float environmentVolume = 0.8f;

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

    public float GetVolume(SoundCategory category)
    {
        float baseVolume = masterVolume;
        switch (category)
        {
            case SoundCategory.BGM:
                baseVolume *= bgmVolume;
                break;
            case SoundCategory.Environment:
                baseVolume *= environmentVolume;
                break;
        }
        return baseVolume;
    }
}

public enum SoundCategory
{
    SFX,
    BGM,
    Environment
}
