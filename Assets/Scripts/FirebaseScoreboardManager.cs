using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FirebaseScoreboardManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI[] scoreTexts;   
    public TextMeshProUGUI loadingText;    
    public Button backButton;              

    [Header("Settings")]
    public int maxScoresToShow = 5;

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
        LoadScores();
    }

    void SetupUI()
    {
        if (backButton != null)
            backButton.onClick.AddListener(GoBackToMainMenu);

        ShowLoadingState(true);
    }

    public void LoadScores()
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
                ShowError("Failed to load scores");
                ShowLoadingState(false);
            }
        );
    }

    void DisplayScores()
    {
        ClearDisplay();

        if (userScores.Count == 0)
        {
            if (scoreTexts != null && scoreTexts.Length > 0 && scoreTexts[0] != null)
            {
                scoreTexts[0].text = "No scores yet!";
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
    }

    void ClearDisplay()
    {
        if (scoreTexts != null)
        {
            foreach (var scoreText in scoreTexts)
            {
                if (scoreText != null) scoreText.text = "";
            }
        }
    }

    void ShowLoadingState(bool isLoading)
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(isLoading);
            if (isLoading)
            {
                loadingText.text = "Loading...";
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
        Debug.Log("Refreshing scores...");
        LoadScores();
    }

    public void GoBackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public int GetBestScore()
    {
        return userScores.Count > 0 ? userScores[0] : 0;
    }
}
