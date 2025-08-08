using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button continueButton;
    public Button newGameButton;
    public Button settingButton;
    public Button scoreBoard;

    public Button exit;

    [Header("Scene Navigation")]
    public string newGameSceneName = "SelectPlayerScene";
    public string settingsSceneName = "SettingScene";
    public string scoreboardSceneName = "ScoreBoard2"; 
    public string gameSceneName = "MultiplayerScene";

    void Start()
    {
        SetupUI();
        CheckContinueButton();
    }

    void SetupUI()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);

        if (settingButton != null)
            settingButton.onClick.AddListener(OpenSettings);

        if (scoreBoard != null)
            scoreBoard.onClick.AddListener(OpenScoreboard);

        if (exit != null)
            exit.onClick.AddListener(CloseApplication);

    }

    void CheckContinueButton()
    {
        bool hasSavedGame = false;
        
        if (SaveSystemManager.Instance != null)
        {
            hasSavedGame = SaveSystemManager.Instance.HasSaveData();
            Debug.Log($"SaveSystemManager.HasSaveData(): {hasSavedGame}");
        }
        else
        {
            hasSavedGame = PlayerPrefs.HasKey("GameInProgress") && PlayerPrefs.GetInt("GameInProgress", 0) == 1;
            Debug.Log($"Fallback PlayerPrefs check: {hasSavedGame}");
        }

        if (continueButton != null)
        {
            continueButton.interactable = hasSavedGame;
            Debug.Log($"Continue button set to interactable: {hasSavedGame}");
        }
        else
        {
            Debug.LogWarning("Continue button reference is null!");
        }

        Debug.Log($"Continue button enabled: {hasSavedGame}");
    }

    public void ContinueGame()
    {
        Debug.Log("Continue Game clicked");
        
        bool hasSaveData = false;
        
        if (SaveSystemManager.Instance != null)
        {
            hasSaveData = SaveSystemManager.Instance.HasSaveData();
            Debug.Log($"SaveSystemManager says has save data: {hasSaveData}");
        }
        
        bool playerPrefsHasSave = PlayerPrefs.GetInt("GameInProgress", 0) == 1;
        
        if (hasSaveData || playerPrefsHasSave)
        {
            PlayerPrefs.SetInt("LoadSavedGame", 1);
            PlayerPrefs.Save();
            Debug.Log("LoadSavedGame flag set to 1");
            
            if (!string.IsNullOrEmpty(gameSceneName))
            {
                Debug.Log($"Loading game scene: {gameSceneName}");
                SceneManager.LoadScene(gameSceneName);
            }
            else
            {
                Debug.LogError("Game scene name not set in MainMenuManager!");
            }
        }
        else
        {
            Debug.LogWarning("No saved game found!");
            Debug.LogWarning("Cannot continue - no save data available");
        }
    }

    public void StartNewGame()
    {
        Debug.Log("New Game clicked");

        PlayerPrefs.SetInt("GameInProgress", 0);
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(newGameSceneName))
        {
            SceneManager.LoadScene(newGameSceneName);
        }
        else
        {
            Debug.LogWarning("New game scene name not set!");
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked");

        if (!string.IsNullOrEmpty(settingsSceneName))
        {
            SceneManager.LoadScene(settingsSceneName);
        }
        else
        {
            Debug.LogWarning("Settings scene name not set!");
        }
    }

    public void OpenScoreboard()
    {
        Debug.Log("[MainMenuManager] Opening scoreboard");

        // Find DatabaseManager to check authentication
        DatabaseManager databaseManager = FindObjectOfType<DatabaseManager>();
        
        if (databaseManager == null)
        {
            Debug.LogError("[MainMenuManager] DatabaseManager not found! Cannot access scoreboard.");
            return;
        }

        // Check if user is authenticated
        if (!databaseManager.IsUserAuthenticated())
        {
            Debug.LogWarning("[MainMenuManager] User not authenticated. Cannot access scoreboard.");
            return;
        }

        if (!string.IsNullOrEmpty(scoreboardSceneName))
        {
            Debug.Log("[MainMenuManager] Loading scoreboard scene: " + scoreboardSceneName);
            SceneManager.LoadScene(scoreboardSceneName);
        }
        else
        {
            Debug.LogError("[MainMenuManager] Scoreboard scene name not set!");
        }
    }

    public void CloseApplication()
    {
        Debug.Log("Quit");
        Application.Quit(0);
    }
}
