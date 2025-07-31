using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// AudioTester - A debugging tool to test volume sliders functionality
/// Attach this to a GameObject in your Settings scene to test audio controls
/// </summary>
public class AudioTester : MonoBehaviour
{
    [Header("Debug UI")]
    public TextMeshProUGUI debugText;
    public Button testSoundButton;
    public Button testMusicButton;
    
    [Header("Test Audio Clips")]
    public AudioClip testSoundClip;
    public AudioClip testMusicClip;
    
    private AudioSource soundTestSource;
    private AudioSource musicTestSource;
    private SettingsManager settingsManager;
    
    void Start()
    {
        // Find the SettingsManager in the scene
        settingsManager = FindObjectOfType<SettingsManager>();
        
        // Create test audio sources
        CreateTestAudioSources();
        
        // Setup test buttons
        SetupTestButtons();
        
        // Start the debug display coroutine
        if (debugText != null)
        {
            StartCoroutine(UpdateDebugDisplay());
        }
        
        Debug.Log("AudioTester initialized - Use this to test your volume sliders!");
    }
    
    void CreateTestAudioSources()
    {
        // Create sound effects test source
        GameObject soundTestGO = new GameObject("SoundTestSource");
        soundTestGO.transform.SetParent(transform);
        soundTestSource = soundTestGO.AddComponent<AudioSource>();
        soundTestSource.playOnAwake = false;
        soundTestSource.volume = 0.5f;
        
        // Create music test source
        GameObject musicTestGO = new GameObject("MusicTestSource");
        musicTestGO.transform.SetParent(transform);
        musicTestSource = musicTestGO.AddComponent<AudioSource>();
        musicTestSource.playOnAwake = false;
        musicTestSource.volume = 0.3f;
        musicTestSource.loop = true;
    }
    
    void SetupTestButtons()
    {
        if (testSoundButton != null)
        {
            testSoundButton.onClick.AddListener(TestSoundEffect);
        }
        
        if (testMusicButton != null)
        {
            testMusicButton.onClick.AddListener(TestMusic);
        }
    }
    
    public void TestSoundEffect()
    {
        Debug.Log("Testing Sound Effect...");
        
        // Use the jump sound from PlayerMovement as test sound if available
        if (testSoundClip == null)
        {
            MultiplayerSceneSetup sceneSetup = FindObjectOfType<MultiplayerSceneSetup>();
            if (sceneSetup != null && sceneSetup.jumpSound != null)
            {
                testSoundClip = sceneSetup.jumpSound;
            }
        }
        
        if (soundTestSource != null && testSoundClip != null)
        {
            // Apply current volume settings
            soundTestSource.volume = AudioListener.volume * 0.7f; // Match player volume
            soundTestSource.PlayOneShot(testSoundClip);
            Debug.Log($"Playing sound effect at volume: {soundTestSource.volume}");
        }
        else
        {
            Debug.LogWarning("No test sound clip available!");
        }
    }
    
    public void TestMusic()
    {
        Debug.Log("Testing Music...");
        
        if (musicTestSource != null)
        {
            if (musicTestSource.isPlaying)
            {
                musicTestSource.Stop();
                Debug.Log("Stopped music test");
            }
            else
            {
                if (testMusicClip != null)
                {
                    // Apply current volume settings
                    musicTestSource.volume = AudioListener.volume * 0.3f; // Lower volume for music
                    musicTestSource.clip = testMusicClip;
                    musicTestSource.Play();
                    Debug.Log($"Playing music at volume: {musicTestSource.volume}");
                }
                else
                {
                    Debug.LogWarning("No test music clip available!");
                }
            }
        }
    }
    
    System.Collections.IEnumerator UpdateDebugDisplay()
    {
        while (true)
        {
            if (debugText != null)
            {
                string debugInfo = GetAudioDebugInfo();
                debugText.text = debugInfo;
            }
            yield return new WaitForSeconds(0.1f); // Update 10 times per second
        }
    }
    
    string GetAudioDebugInfo()
    {
        string info = "=== AUDIO DEBUG INFO ===\n\n";
        
        // Main audio settings
        info += $"AudioListener.volume: {AudioListener.volume:F2}\n";
        info += $"Time.timeScale: {Time.timeScale:F2}\n\n";
        
        // SettingsManager status
        if (settingsManager != null)
        {
            info += "SettingsManager: FOUND\n";
            // Check if volume slider is assigned
            info += $"Volume Slider: {(settingsManager.volumeSlider != null ? "ASSIGNED" : "NULL")}\n";
            if (settingsManager.volumeSlider != null)
            {
                info += $"Slider Value: {settingsManager.volumeSlider.value:F2}\n";
                info += $"Slider Min/Max: {settingsManager.volumeSlider.minValue:F1}/{settingsManager.volumeSlider.maxValue:F1}\n";
            }
        }
        else
        {
            info += "SettingsManager: NOT FOUND\n";
        }
        
        info += "\n=== TEST SOURCES ===\n";
        info += $"Sound Test Source: {(soundTestSource != null ? $"Vol={soundTestSource.volume:F2}" : "NULL")}\n";
        info += $"Music Test Source: {(musicTestSource != null ? $"Vol={musicTestSource.volume:F2}, Playing={musicTestSource.isPlaying}" : "NULL")}\n";
        
        // PlayerPrefs volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            info += $"\nSaved Volume: {PlayerPrefs.GetFloat("Volume"):F2}\n";
        }
        else
        {
            info += "\nSaved Volume: NOT SAVED\n";
        }
        
        info += "\n=== CONTROLS ===\n";
        info += "• Adjust sliders to test volume\n";
        info += "• Click buttons to test audio\n";
        info += "• Check console for detailed logs";
        
        return info;
    }
    
    void Update()
    {
        // Keyboard shortcuts for quick testing
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestSoundEffect();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            TestMusic();
        }
        
        // Volume adjustment with keyboard (for testing without sliders)
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            AudioListener.volume = Mathf.Clamp01(AudioListener.volume + 0.1f);
            Debug.Log($"Volume increased to: {AudioListener.volume:F2}");
        }
        
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            AudioListener.volume = Mathf.Clamp01(AudioListener.volume - 0.1f);
            Debug.Log($"Volume decreased to: {AudioListener.volume:F2}");
        }
    }
    
    void OnDestroy()
    {
        // Cleanup
        if (soundTestSource != null) Destroy(soundTestSource.gameObject);
        if (musicTestSource != null) Destroy(musicTestSource.gameObject);
    }
}
