using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip photoSfx;
    public AudioClip flossDanceSfx;
    public AudioClip hypeDanceSfx;
    public AudioClip selfCheckSfx;
    public AudioClip ignoreSfx;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Plays a sound effect
    public void Play(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Stops all current SFX
    public void Stop()
    {
        sfxSource?.Stop();
    }
}
