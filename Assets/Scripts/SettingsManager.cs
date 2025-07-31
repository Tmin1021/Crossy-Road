using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Button closeButton;
    public Slider volumeSlider;
    
    [Header("Player 1 Key Binding Buttons")]
    public Button player1LeftButton;
    public Button player1RightButton;
    public Button player1UpButton;
    public Button player1DownButton;
    
    [Header("Player 2 Key Binding Buttons")]
    public Button player2LeftButton;
    public Button player2RightButton;
    public Button player2UpButton;
    public Button player2DownButton;
    
    [Header("Key Display Texts")]
    public TextMeshProUGUI player1LeftText;
    public TextMeshProUGUI player1RightText;
    public TextMeshProUGUI player1UpText;
    public TextMeshProUGUI player1DownText;
    public TextMeshProUGUI player2LeftText;
    public TextMeshProUGUI player2RightText;
    public TextMeshProUGUI player2UpText;
    public TextMeshProUGUI player2DownText;
    
    [Header("Scene Navigation")]
    public string gameSceneName = "SelectCharacterScene"; 
    
    private bool isWaitingForKey = false;
    private string currentKeyToChange = "";
    
    private KeyCode player1Left = KeyCode.A;
    private KeyCode player1Right = KeyCode.D;
    private KeyCode player1Up = KeyCode.W;
    private KeyCode player1Down = KeyCode.S;
    
    private KeyCode player2Left = KeyCode.LeftArrow;
    private KeyCode player2Right = KeyCode.RightArrow;
    private KeyCode player2Up = KeyCode.UpArrow;
    private KeyCode player2Down = KeyCode.DownArrow;

    void Start()
    {
        if (Application.isPlaying && settingsPanel != null)
        {
            LoadSettings();
            SetupUI();
            UpdateKeyDisplays();
        }
    }

    void SetupUI()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (player1LeftButton != null)
        {
            player1LeftButton.onClick.AddListener(() => StartKeyBinding("player1Left"));
        }
        if (player1RightButton != null)
        {
            player1RightButton.onClick.AddListener(() => StartKeyBinding("player1Right"));
        }
        if (player1UpButton != null)
        {
            player1UpButton.onClick.AddListener(() => StartKeyBinding("player1Up"));
        }
        if (player1DownButton != null)
        {
            player1DownButton.onClick.AddListener(() => StartKeyBinding("player1Down"));
        }

        if (player2LeftButton != null)
        {
            player2LeftButton.onClick.AddListener(() => StartKeyBinding("player2Left"));
        }
        if (player2RightButton != null)
        {
            player2RightButton.onClick.AddListener(() => StartKeyBinding("player2Right"));
        }
        if (player2UpButton != null)
        {
            player2UpButton.onClick.AddListener(() => StartKeyBinding("player2Up"));
        }
        if (player2DownButton != null)
        {
            player2DownButton.onClick.AddListener(() => StartKeyBinding("player2Down"));
        }
    }

    void Update()
    {
        if (Application.isPlaying && isWaitingForKey)
        {
            HandleKeyInput();
        }
        
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestVolumeSliderSetup();
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log($"Quick Volume Check - AudioListener: {AudioListener.volume:F2}, Slider: {(volumeSlider != null ? volumeSlider.value.ToString("F2") : "NULL")}");
        }
        #endif
    }

    void StartKeyBinding(string keyToChange)
    {
        isWaitingForKey = true;
        currentKeyToChange = keyToChange;
        
        UpdateButtonText(keyToChange, "Press any key...");
    }

    void HandleKeyInput()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                SetKeyBinding(currentKeyToChange, keyCode);
                isWaitingForKey = false;
                currentKeyToChange = "";
                UpdateKeyDisplays();
                SaveSettings();
                break;
            }
        }
    }

    void SetKeyBinding(string keyName, KeyCode keyCode)
    {
        switch (keyName)
        {
            case "player1Left":
                player1Left = keyCode;
                break;
            case "player1Right":
                player1Right = keyCode;
                break;
            case "player1Up":
                player1Up = keyCode;
                break;
            case "player1Down":
                player1Down = keyCode;
                break;
            case "player2Left":
                player2Left = keyCode;
                break;
            case "player2Right":
                player2Right = keyCode;
                break;
            case "player2Up":
                player2Up = keyCode;
                break;
            case "player2Down":
                player2Down = keyCode;
                break;
        }
        
        UpdatePlayerMovementKeys();
    }

    void UpdatePlayerMovementKeys()
    {
        if (!Application.isPlaying) return;
        
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement player in players)
        {
            if (player.playerID == 1)
            {
                player.leftKey = player1Left;
                player.rightKey = player1Right;
                player.upKey = player1Up;
                player.downKey = player1Down;
            }
            else if (player.playerID == 2)
            {
                player.leftKey = player2Left;
                player.rightKey = player2Right;
                player.upKey = player2Up;
                player.downKey = player2Down;
            }
        }
    }

    string FormatKeyName(KeyCode key)
    {
        string keyName = key.ToString();
        if (keyName.EndsWith("Arrow"))
        {
            return keyName.Replace("Arrow", "");
        }
        int spaceIndex = keyName.IndexOf(' ');
        if (spaceIndex > 0)
        {
            return keyName.Substring(0, spaceIndex);
        }
        
        return keyName;
    }

    void UpdateKeyDisplays()
    {
        // Debug.Log("UpdateKeyDisplays called");
        
        if (player1LeftText != null) 
        {
            player1LeftText.text = FormatKeyName(player1Left);
            // Debug.Log($"Updated P1 Left: {player1Left}");
        }
        // else Debug.Log("player1LeftText is null!");
        
        if (player1RightText != null) player1RightText.text = FormatKeyName(player1Right);
        if (player1UpText != null) player1UpText.text = FormatKeyName(player1Up);
        if (player1DownText != null) player1DownText.text = FormatKeyName(player1Down);
        
        if (player2LeftText != null) player2LeftText.text = FormatKeyName(player2Left);
        if (player2RightText != null) player2RightText.text = FormatKeyName(player2Right);
        if (player2UpText != null) player2UpText.text = FormatKeyName(player2Up);
        if (player2DownText != null) player2DownText.text = FormatKeyName(player2Down);
    }

    void UpdateButtonText(string keyName, string text)
    {
        switch (keyName)
        {
            case "player1Left":
                if (player1LeftText != null) player1LeftText.text = text;
                break;
            case "player1Right":
                if (player1RightText != null) player1RightText.text = text;
                break;
            case "player1Up":
                if (player1UpText != null) player1UpText.text = text;
                break;
            case "player1Down":
                if (player1DownText != null) player1DownText.text = text;
                break;
            case "player2Left":
                if (player2LeftText != null) player2LeftText.text = text;
                break;
            case "player2Right":
                if (player2RightText != null) player2RightText.text = text;
                break;
            case "player2Up":
                if (player2UpText != null) player2UpText.text = text;
                break;
            case "player2Down":
                if (player2DownText != null) player2DownText.text = text;
                break;
        }
    }

    void OnVolumeChanged(float value)
    {
        // Only change volume in play mode
        if (!Application.isPlaying) return;
        
        AudioListener.volume = value;
        SaveSettings();
        
        // Debug output to verify slider is working
        Debug.Log($"Volume slider changed to: {value:F2} | AudioListener.volume: {AudioListener.volume:F2}");
    }
    
    // Call this method to test your volume slider setup
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void TestVolumeSliderSetup()
    {
        Debug.Log("=== VOLUME SLIDER TEST ===");
        Debug.Log($"SettingsManager found: {this != null}");
        Debug.Log($"Volume Slider assigned: {volumeSlider != null}");
        
        if (volumeSlider != null)
        {
            Debug.Log($"Slider Value: {volumeSlider.value:F2}");
            Debug.Log($"Slider Min/Max: {volumeSlider.minValue:F1} to {volumeSlider.maxValue:F1}");
            Debug.Log($"Slider Interactable: {volumeSlider.interactable}");
        }
        
        Debug.Log($"Current AudioListener.volume: {AudioListener.volume:F2}");
        Debug.Log($"Saved Volume in PlayerPrefs: {PlayerPrefs.GetFloat("Volume", -1):F2}");
        
        // Test volume change
        float testVolume = 0.5f;
        if (volumeSlider != null)
        {
            volumeSlider.value = testVolume;
            Debug.Log($"Set slider to test volume: {testVolume}");
        }
        
        Debug.Log("=== END TEST ===");
    }

    public void OpenSettings()
    {
        if (!Application.isPlaying) return;
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void CloseSettings()
    {
        if (!Application.isPlaying) return;
        
        SaveSettings();
        
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Time.timeScale = 1f; 
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
                Time.timeScale = 1f; 
            }
        }
    }

    void SaveSettings()
    {
        PlayerPrefs.SetString("Player1Left", player1Left.ToString());
        PlayerPrefs.SetString("Player1Right", player1Right.ToString());
        PlayerPrefs.SetString("Player1Up", player1Up.ToString());
        PlayerPrefs.SetString("Player1Down", player1Down.ToString());
        
        PlayerPrefs.SetString("Player2Left", player2Left.ToString());
        PlayerPrefs.SetString("Player2Right", player2Right.ToString());
        PlayerPrefs.SetString("Player2Up", player2Up.ToString());
        PlayerPrefs.SetString("Player2Down", player2Down.ToString());
        
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
        
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        if (!Application.isPlaying) return;
        
        if (PlayerPrefs.HasKey("Player1Left"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player1Left"), out player1Left);
        if (PlayerPrefs.HasKey("Player1Right"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player1Right"), out player1Right);
        if (PlayerPrefs.HasKey("Player1Up"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player1Up"), out player1Up);
        if (PlayerPrefs.HasKey("Player1Down"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player1Down"), out player1Down);
            
        if (PlayerPrefs.HasKey("Player2Left"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player2Left"), out player2Left);
        if (PlayerPrefs.HasKey("Player2Right"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player2Right"), out player2Right);
        if (PlayerPrefs.HasKey("Player2Up"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player2Up"), out player2Up);
        if (PlayerPrefs.HasKey("Player2Down"))
            System.Enum.TryParse(PlayerPrefs.GetString("Player2Down"), out player2Down);
        
        if (PlayerPrefs.HasKey("Volume"))
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        
        UpdatePlayerMovementKeys();
    }

    public KeyCode GetPlayer1LeftKey() => player1Left;
    public KeyCode GetPlayer1RightKey() => player1Right;
    public KeyCode GetPlayer1UpKey() => player1Up;
    public KeyCode GetPlayer1DownKey() => player1Down;
    
    public KeyCode GetPlayer2LeftKey() => player2Left;
    public KeyCode GetPlayer2RightKey() => player2Right;
    public KeyCode GetPlayer2UpKey() => player2Up;
    public KeyCode GetPlayer2DownKey() => player2Down;
}
