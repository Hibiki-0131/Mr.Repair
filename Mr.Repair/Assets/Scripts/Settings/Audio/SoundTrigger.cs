using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [SerializeField] private SoundDatabase database;

    public void PlayByKey(string key, Vector3 position)
    {
        if (database == null)
        {
            Debug.LogError("SoundTrigger: SoundDatabase が設定されていません。");
            return;
        }

        var sound = database.GetSound(key);
        if (sound == null)
        {
            Debug.LogWarning($"SoundTrigger: '{key}' に対応する音がデータベースに存在しません。");
            return;
        }

        SoundPlayer.Instance.PlaySound(sound.clip, sound.category, position);
    }
}
