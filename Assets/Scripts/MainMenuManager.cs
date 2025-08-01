using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
    public string settingsSceneName = "SettingsScene";
    public string scoreboardSceneName = "ScoreboardScene";
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
        Debug.Log("Scoreboard clicked");

        if (!string.IsNullOrEmpty(scoreboardSceneName))
        {
            SceneManager.LoadScene(scoreboardSceneName);
        }
        else
        {
            Debug.LogWarning("Scoreboard scene name not set!");
        }
    }

    public void CloseApplication()
    {
        Debug.Log("Quit");
        Application.Quit(0);
    }
    // void Start()
    // {
    //     onePlayerButton.onClick.AddListener(() => SelectMode(1));
    //     twoPlayerButton.onClick.AddListener(() => SelectMode(2));
    //     nextScreenButton.onClick.AddListener(StartGame);

    //     SelectMode(1);
    // }

    // public void SelectMode(int mode)
    // {
    //     selectedMode = mode;

    //     onePlayerButton.interactable = (mode != 1);
    //     twoPlayerButton.interactable = (mode != 2);
    // }

    // public void OnCharacterChanged(int characterIndex)
    // {
    //     selectedCharacterIndex = characterIndex;
    //     Debug.Log($"Selected character index: {characterIndex}");
    // }

    // public void StartGame()
    // {
    //     PlayerPrefs.SetInt("SelectedGameMode", selectedMode);
    //     PlayerPrefs.SetInt("IsTwoPlayerMode", selectedMode == 2 ? 1 : 0);

    //     if (characterManager != null)
    //     {
    //         selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
    //     }
    //     PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
    //     PlayerPrefs.SetInt("Player1Character", selectedCharacterIndex);

    //     if (selectedMode == 2)
    //     {
    //         PlayerPrefs.SetInt("Player2Character", selectedCharacterIndex); 
    //     }


    //     PlayerPrefs.Save();

    //     Debug.Log($"Starting game: Mode={selectedMode}P, Character={selectedCharacterIndex}");
    //     SceneManager.LoadScene(playSceneName);
    // }
}
