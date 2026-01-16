using UnityEngine;
using System.Collections.Generic;

public class InstantAudioSource : MonoBehaviour
{
    public static InstantAudioSource Instance { get; private set; }

    [SerializeField] private GameObject audioSourcePrefab;

    // 現在再生中のAudioSourceとそのカテゴリを追跡するためのリスト
    private List<(AudioSource source, SoundCategory category)> activeSources = new List<(AudioSource, SoundCategory)>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // SoundManagerの音量変更イベントを購読
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeChanged += UpdateAllActiveVolumes;
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

        // リストに追加
        var entry = (source, category);
        activeSources.Add(entry);

        // 再生終了後にリストから削除して破壊
        float duration = clip.length;
        StartCoroutine(DestroyAndRemove(obj, source, duration));
    }

    private System.Collections.IEnumerator DestroyAndRemove(GameObject obj, AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeSources.RemoveAll(x => x.source == source);
        Destroy(obj);
    }

    // 音量が変更されたときに、再生中の全AudioSourceの音量を更新する
    private void UpdateAllActiveVolumes()
    {
        // リストから無効になった（既に破壊された）ものを掃除しつつ更新
        activeSources.RemoveAll(x => x.source == null);

        foreach (var item in activeSources)
        {
            if (item.source != null)
            {
                item.source.volume = SoundManager.Instance.GetVolume(item.category);
            }
        }
    }
}