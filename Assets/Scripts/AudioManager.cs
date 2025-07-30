using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource backgroundMusicSource;   // For background music
    public AudioSource sfxSource;               // For sound effects
    public AudioClip backgroundMusicClip;       // Background music clip
    public AudioClip[] soundEffects;            // Array of sound effects

    void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep AudioManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Play background music
        PlayBackgroundMusic();
    }

    // Play background music
    public void PlayBackgroundMusic()
    {
        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.Play();
    }

    // Play a sound effect by index
    public void PlaySoundEffect(int index)
    {
        if (index < 0 || index >= soundEffects.Length) return;  // Ensure index is valid

        sfxSource.PlayOneShot(soundEffects[index]);
    }
}
