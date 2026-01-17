using UnityEngine;
using System.Collections.Generic;

public class InstantAudioSource : MonoBehaviour
{
    public static InstantAudioSource Instance { get; private set; }

    [SerializeField] private GameObject audioSourcePrefab;
    private List<(AudioSource source, SoundCategory category)> activeSources = new List<(AudioSource, SoundCategory)>();

    // ★追加：前回の更新タイミングを記録する変数
    private int lastUpdateCount = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ★修正：Startでのイベント登録を削除し、Updateで監視する
    private void Update()
    {
        if (SoundManager.Instance == null) return;

        // SoundManager側のカウンターが増えていたら音量を一斉更新
        if (lastUpdateCount != SoundManager.Instance.VolumeUpdateCount)
        {
            lastUpdateCount = SoundManager.Instance.VolumeUpdateCount;
            UpdateAllActiveVolumes();
        }
    }

    public void PlaySound(AudioClip clip, SoundCategory category, Vector3 position)
    {
        if (clip == null) return;

        GameObject obj = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        AudioSource source = obj.GetComponent<AudioSource>();

        float volume = SoundManager.Instance.GetVolume(category);

        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f;
        source.Play();

        activeSources.Add((source, category));
        StartCoroutine(DestroyAndRemove(obj, source, clip.length));
    }

    private System.Collections.IEnumerator DestroyAndRemove(GameObject obj, AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeSources.RemoveAll(x => x.source == source);
        Destroy(obj);
    }

    private void UpdateAllActiveVolumes()
    {
        activeSources.RemoveAll(x => x.source == null);

        foreach (var item in activeSources)
        {
            if (item.source != null)
            {
                item.source.volume = SoundManager.Instance.GetVolume(item.category);
            }
        }
        // デバッグ用（動いたら消してOK）
        Debug.Log($"再生中の {activeSources.Count} 個のSE音量を更新しました");
    }
}