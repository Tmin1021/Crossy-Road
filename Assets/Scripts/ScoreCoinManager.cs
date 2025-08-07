using UnityEngine;
using UnityEngine.UI;

public class ScoreCoinManager : MonoBehaviour
{
    public Text scoreText;
    public Text coinText;
    private int score;
    private int coins;
    private DatabaseManager databaseManager;
    void Start()
    {
        score = 0;
        coins = 0; 
        
        // Find DatabaseManager
        databaseManager = FindObjectOfType<DatabaseManager>();
        
        if (PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Load coins from Firebase
        LoadCoinsFromDatabase();
        
        UpdateScoreText();
        UpdateCoinText();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }
    public void IncreaseCoin(int amount)
    {
        coins += amount;
        UpdateCoinText();
    }

    // public void SetScore(int newScore)
    // {
    //     score = newScore;
    //     UpdateScoreText();
    // }

    public int GetCurrentScore()
    {
        return score;
    }
    public int GetCurrentCoins()
    {
        return coins;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
    
    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = " Coins: " + coins.ToString();
        }
    }

    void LoadCoinsFromDatabase()
    {
        if (databaseManager == null || !databaseManager.IsUserAuthenticated())
        {
            return;
        }

        databaseManager.ReadUserCoins(
            onCoinsLoaded: (loadedCoins) => {
                coins = loadedCoins;
                UpdateCoinText();
            },
            onError: (error) => {
                Debug.LogError($"Failed to load coins: {error}");
            }
        );
    }

    public void SaveCoinsToDatabase()
    {
        if (databaseManager == null || !databaseManager.IsUserAuthenticated())
        {
            return;
        }

        databaseManager.SaveUserData(coins, score);
    }
}
