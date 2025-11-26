using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Audio/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip clip;
        public SoundCategory category = SoundCategory.SFX;
    }

    public List<SoundEntry> sounds = new List<SoundEntry>();
    private Dictionary<string, SoundEntry> soundDict;

    private void OnEnable()
    {
        soundDict = new Dictionary<string, SoundEntry>();
        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.key))
                soundDict.Add(s.key, s);
        }
    }

    public SoundEntry GetSound(string key)
    {
        if (soundDict != null && soundDict.ContainsKey(key))
            return soundDict[key];
        return null;
    }
}
