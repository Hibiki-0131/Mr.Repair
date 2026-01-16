using UnityEngine;
using System; // Actionを使うために必要

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // 音量が変更されたことを他のスクリプトに通知するイベント
    public event Action OnVolumeChanged;

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

    // --- ここが重要：UIから呼び出されるメソッド群 ---

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        OnVolumeChanged?.Invoke(); // 「変わったよ！」と通知
    }

    public void SetBgmVolume(float value)
    {
        bgmVolume = value;
        OnVolumeChanged?.Invoke(); // 「変わったよ！」と通知
    }

    public void SetEnvVolume(float value)
    {
        environmentVolume = value;
        OnVolumeChanged?.Invoke(); // 「変わったよ！」と通知
    }

    // --- 既存の取得メソッド ---
    public float GetVolume(SoundCategory category)
    {
        float baseVolume = masterVolume;
        switch (category)
        {
            case SoundCategory.BGM:
                baseVolume *= bgmVolume;
                break;
            case SoundCategory.Environment:
            case SoundCategory.SFX: // SFXもEnvironmentと同じ変数で制御するようにする
                baseVolume *= environmentVolume;
                break;
        }
        return baseVolume;
    }
}