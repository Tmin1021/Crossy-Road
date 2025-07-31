# Volume Slider Testing Guide

## Quick Tests You Can Do Right Now:

### 1. **Console Testing (Immediate)**
```csharp
// Add this to any Update() method in an active script to test volume changes:
if (Input.GetKeyDown(KeyCode.V))
{
    Debug.Log($"Current AudioListener.volume: {AudioListener.volume}");
    Debug.Log($"PlayerPrefs Volume: {PlayerPrefs.GetFloat("Volume", -1)}");
}
```

### 2. **Manual Slider Testing in Unity Editor**
1. Open the Settings Scene
2. Find your SettingsManager GameObject in the hierarchy
3. In the Inspector, check if:
   - `volumeSlider` field is assigned
   - The slider GameObject exists in the scene
4. Play the scene and move the slider - check console for volume changes

### 3. **Runtime Volume Testing**
```csharp
// Add this to test volume changes in code:
public void TestVolumeChange()
{
    float testVolume = 0.5f;
    AudioListener.volume = testVolume;
    Debug.Log($"Set volume to: {testVolume}");
    
    // Test if PlayerPrefs saves correctly
    PlayerPrefs.SetFloat("Volume", testVolume);
    PlayerPrefs.Save();
    Debug.Log("Saved volume to PlayerPrefs");
}
```

## Using the AudioTester Script:

### Setup:
1. Create an empty GameObject in your Settings scene
2. Name it "AudioTester"
3. Attach the AudioTester.cs script to it
4. Assign UI elements if you want visual feedback:
   - Create a TextMeshPro text for debug display
   - Create buttons for testing sound/music

### Testing Features:
- **Real-time Debug Display**: Shows current volume, slider values, and settings status
- **Keyboard Shortcuts**:
  - `T` key: Test sound effect
  - `M` key: Test music
  - `+/=` key: Increase volume
  - `-` key: Decrease volume
- **Automatic Audio Source Creation**: Creates test audio sources for testing

## Debugging Checklist:

### ✅ **SettingsManager Setup**
- [ ] SettingsManager script is in the scene
- [ ] volumeSlider field is assigned in inspector
- [ ] Slider GameObject exists and is active
- [ ] OnVolumeChanged method is connected to slider

### ✅ **UI Slider Setup**
- [ ] Slider has correct Min Value (usually 0)
- [ ] Slider has correct Max Value (usually 1)
- [ ] Slider Value matches current AudioListener.volume
- [ ] Slider is interactable (not disabled)

### ✅ **Audio System**
- [ ] AudioListener exists in scene (usually on Camera)
- [ ] AudioListener.volume changes when slider moves
- [ ] Volume is saved to PlayerPrefs
- [ ] Volume is loaded from PlayerPrefs on scene start

### ✅ **Testing Sounds**
- [ ] Test with jump sound from PlayerMovement
- [ ] Test with death sound from PlayerMovement
- [ ] Test with any other AudioSource in scene

## Common Issues & Solutions:

### **Slider Not Responding**
```csharp
// Check if event listener is properly connected:
if (volumeSlider != null)
{
    volumeSlider.onValueChanged.RemoveAllListeners();
    volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    Debug.Log("Reconnected volume slider listener");
}
```

### **Volume Not Saving**
```csharp
// Ensure PlayerPrefs.Save() is called:
void OnVolumeChanged(float value)
{
    AudioListener.volume = value;
    PlayerPrefs.SetFloat("Volume", value);
    PlayerPrefs.Save(); // Don't forget this!
    Debug.Log($"Volume changed to: {value}");
}
```

### **No Audio Playing**
```csharp
// Check AudioSource setup:
AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
Debug.Log($"Found {allAudioSources.Length} AudioSources in scene");
foreach (var source in allAudioSources)
{
    Debug.Log($"AudioSource on {source.name}: volume={source.volume}, clip={source.clip?.name}");
}
```

## Testing Procedure:

1. **Start Unity and open Settings Scene**
2. **Add AudioTester to scene** (optional but recommended)
3. **Play the scene**
4. **Check Console** for initialization messages
5. **Move volume slider** and watch console for changes
6. **Test with actual sounds**:
   - Go to game scene and test player sounds
   - Or use AudioTester buttons
7. **Check persistence**:
   - Change volume, close scene, reopen
   - Volume should be remembered

## Expected Console Output:
```
AudioTester initialized - Use this to test your volume sliders!
SettingsManager found: True
Volume Slider assigned: True
Current AudioListener.volume: 0.75
Volume changed to: 0.5
Saved volume to PlayerPrefs
Playing sound effect at volume: 0.5
```
