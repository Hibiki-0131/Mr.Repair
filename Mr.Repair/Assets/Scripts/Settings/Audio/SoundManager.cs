using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // 音量が変更されたときに通知するイベント
    public event Action OnVolumeChanged;

    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.8f;
    [Range(0f, 1f)] public float environmentVolume = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // スライダーから呼ばれるメソッド
    public void SetMasterVolume(float value) { masterVolume = value; OnVolumeChanged?.Invoke(); }
    public void SetBgmVolume(float value) { bgmVolume = value; OnVolumeChanged?.Invoke(); }
    public void SetEnvVolume(float value) { environmentVolume = value; OnVolumeChanged?.Invoke(); }

    public float GetVolume(SoundCategory category)
    {
        float baseVolume = masterVolume;
        switch (category)
        {
            case SoundCategory.BGM: baseVolume *= bgmVolume; break;
            case SoundCategory.Environment: baseVolume *= environmentVolume; break;
        }
        return baseVolume;
    }
}