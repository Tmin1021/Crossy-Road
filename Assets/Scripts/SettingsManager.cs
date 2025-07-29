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
    public string gameSceneName = "SampleScene"; // Name of your game scene
    
    private bool isWaitingForKey = false;
    private string currentKeyToChange = "";
    
    // Default key bindings
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
        // Only initialize if we're in play mode and have a settings panel
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
        
        // Update PlayerMovement scripts with new key bindings
        UpdatePlayerMovementKeys();
    }

    void UpdatePlayerMovementKeys()
    {
        // Only update in play mode
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

    void UpdateKeyDisplays()
    {
        Debug.Log("UpdateKeyDisplays called");
        
        if (player1LeftText != null) 
        {
            player1LeftText.text = player1Left.ToString();
            Debug.Log($"Updated P1 Left: {player1Left}");
        }
        else Debug.Log("player1LeftText is null!");
        
        if (player1RightText != null) player1RightText.text = player1Right.ToString();
        if (player1UpText != null) player1UpText.text = player1Up.ToString();
        if (player1DownText != null) player1DownText.text = player1Down.ToString();
        
        if (player2LeftText != null) player2LeftText.text = player2Left.ToString();
        if (player2RightText != null) player2RightText.text = player2Right.ToString();
        if (player2UpText != null) player2UpText.text = player2Up.ToString();
        if (player2DownText != null) player2DownText.text = player2Down.ToString();
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
            Time.timeScale = 1f; // Make sure time scale is normal
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
                Time.timeScale = 1f; // Resume the game
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
        
        // Save volume
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
