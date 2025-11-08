using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [SerializeField] private SoundDatabase database;
    [SerializeField] private string soundKey;
    [SerializeField] private bool playOnStart = false;

    private void Start()
    {
        if (playOnStart) PlayAt(transform.position);
    }

    public void PlayAt(Vector3 position)
    {
        var sound = database.GetSound(soundKey);
        if (sound != null)
        {
            SoundPlayer.Instance.PlaySound(sound.clip, sound.category, position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayAt(transform.position);
    }
}
