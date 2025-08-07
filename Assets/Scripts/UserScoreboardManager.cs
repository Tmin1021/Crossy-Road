using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UserScoreboardManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI[] scoreTexts; 
    public Button[] scoreButtons;
    public TextMeshProUGUI userCoinsText;
    public TextMeshProUGUI loadingText; 
    public Button refreshButton; 
    public Button backButton; 

    [Header("Score Display Settings")]
    public int maxScoresToShow = 5; 
    public string noScoresMessage = "No scores yet!";
    public string loadingMessage = "Loading scores...";

    private DatabaseManager databaseManager;
    private List<int> userScores = new List<int>();

    void Start()
    {
        
        databaseManager = FindObjectOfType<DatabaseManager>();
        
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager not found in scene!");
            return;
        }

        SetupUI();
        LoadUserData();
    }

    void SetupUI()
    {
        
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshScores);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBackToMainMenu);
        }

        ShowLoadingState(true);
    }

    public void LoadUserData()
    {
        ShowLoadingState(true);
        
        databaseManager.ReadUserScoreHistory(
            onScoresLoaded: (scores) => {
                userScores = scores;
                DisplayScores();
                ShowLoadingState(false);
            },
            onError: (error) => {
                Debug.LogError("Failed to load scores: " + error);
                ShowError("Failed to load scores: " + error);
                ShowLoadingState(false);
            }
        );

        // // Load user coins
        // databaseManager.ReadUserCoins(
        //     onCoinsLoaded: (coins) => {
        //         DisplayCoins(coins);
        //     },
        //     onError: (error) => {
        //         Debug.LogError("Failed to load coins: " + error);
        //         if (userCoinsText != null)
        //         {
        //             userCoinsText.text = "Coins: Error";
        //         }
        //     }
        // );
    }

    void DisplayScores()
    {
        if (scoreTexts == null || scoreTexts.Length == 0)
        {
            Debug.LogWarning("Score text elements not assigned!");
            return;
        }

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (scoreTexts[i] != null)
            {
                scoreTexts[i].text = "";
            }
        }

        if (userScores.Count == 0)
        {
            if (scoreTexts[0] != null)
            {
                scoreTexts[0].text = noScoresMessage;
            }
            return;
        }

        int scoresToShow = Mathf.Min(userScores.Count, scoreTexts.Length, maxScoresToShow);
        
        for (int i = 0; i < scoresToShow; i++)
        {
            if (scoreTexts[i] != null)
            {
                scoreTexts[i].text = $"#{i + 1}: {userScores[i]}";
            }
        }

        Debug.Log($"Displayed {scoresToShow} scores out of {userScores.Count} total scores");
    }

    void DisplayCoins(int coins)
    {
        if (userCoinsText != null)
        {
            userCoinsText.text = $"Coins: {coins}";
        }
    }

    void ShowLoadingState(bool isLoading)
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(isLoading);
            if (isLoading)
            {
                loadingText.text = loadingMessage;
            }
        }
    }

    void ShowError(string errorMessage)
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            loadingText.text = errorMessage;
        }
    }

    public void RefreshScores()
    {
        Debug.Log("Refreshing user scores...");
        LoadUserData();
    }

    public void GoBackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
