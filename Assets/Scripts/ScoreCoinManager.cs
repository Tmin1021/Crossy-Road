using UnityEngine;
using UnityEngine.UI;

public class ScoreCoinManager : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text coinText;
    
    [Header("Auto-Save Settings")]
    public bool autoSaveOnCoinChange = true;
    public float saveDelay = 2f; 
    
    private int score;
    private int coins;
    private int totalCoinsEarned; 
    private DatabaseManager databaseManager;
    private float lastCoinChangeTime;
    private bool hasPendingSave;
    
    void Start()
    {
        score = 0;
        coins = 0;
        totalCoinsEarned = 0;
        
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
        }
        
        if (databaseManager == null)
        {
            Debug.LogWarning("[ScoreCoinManager] DatabaseManager not found - data will not be persisted to Firebase");
        }
        else
        {
            Debug.Log("[ScoreCoinManager] DatabaseManager found and connected");
            Debug.Log($"[ScoreCoinManager] User authenticated: {databaseManager.IsUserAuthenticated()}");
        }
        
        if (PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1)
        {
            Debug.Log("[ScoreCoinManager] Two-player mode detected - disabling score/coin tracking and UI");
            
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(false);
            }
            
            if (coinText != null)
            {
                coinText.gameObject.SetActive(false);
            }
            
            this.enabled = false;
            return;
        }

        UpdateScoreText();
        UpdateCoinText();
    }
    
    void Update()
    {
        if (!this.enabled)
            return;
            
        if (hasPendingSave && Time.time - lastCoinChangeTime >= saveDelay)
        {
            SaveProgressToDatabase();
            hasPendingSave = false;
        }
    }

    public void IncreaseScore(int amount)
    {
        if (!this.enabled)
            return;
            
        score += amount;
        Debug.Log($"[ScoreCoinManager] Score increased by {amount}, total score: {score}");
        UpdateScoreText();
        
        if (databaseManager != null)
        {
            Debug.Log("[ScoreCoinManager] Score increased - saving to database immediately");
            SaveProgressToDatabase();
        }
        else
        {
            Debug.LogWarning("[ScoreCoinManager] DatabaseManager is null, cannot save score increase");
        }
    }
    
    public void IncreaseCoin(int amount)
    {
        if (!this.enabled)
            return;
            
        coins += amount;
        totalCoinsEarned += amount;
        UpdateCoinText();
        
        if (autoSaveOnCoinChange)
        {
            lastCoinChangeTime = Time.time;
            hasPendingSave = true;
            Debug.Log($"[ScoreCoinManager] Auto-save scheduled in {saveDelay} seconds");
        }
    }

    public void SetCoins(int newCoins)
    {
        if (!this.enabled)
            return;
            
        coins = newCoins;
        UpdateCoinText();
    }

    public int GetCurrentScore()
    {
        return score;
    }
    
    public int GetCurrentCoins()
    {
        return coins;
    }
    
    public int GetTotalCoinsEarned()
    {
        return totalCoinsEarned;
    }
    
    public void SaveProgressToDatabase()
    {
        Debug.Log($"[ScoreCoinManager] SaveProgressToDatabase called - Score: {score}, TotalCoinsEarned: {totalCoinsEarned}");
        
        if (databaseManager == null)
        {
            Debug.LogWarning("[ScoreCoinManager] Cannot save to database - DatabaseManager not available, saving locally only");
            SaveLocalProgress();
            return;
        }
        
        Debug.Log($"[ScoreCoinManager] DatabaseManager found, checking authentication...");
        
        bool isAuthenticated = databaseManager.IsUserAuthenticated();
        Debug.Log($"[ScoreCoinManager] User authentication status: {isAuthenticated}");
        
        if (!isAuthenticated)
        {
            Debug.LogWarning("[ScoreCoinManager] Cannot save to database - User not authenticated, saving locally only");
            SaveLocalProgress();
            return;
        }
        
        int existingCoins = PlayerPrefs.GetInt("PlayerCoins", 80);
        int updatedCoins = existingCoins + totalCoinsEarned;
        
        Debug.Log($"[ScoreCoinManager] Saving to Firebase - ExistingCoins: {existingCoins}, EarnedCoins: {totalCoinsEarned}, UpdatedCoins: {updatedCoins}, Score: {score}");
        
        // Save both score and updated total coins to Firebase
        databaseManager.SaveUserData(updatedCoins, score);
        
        // Update local PlayerPrefs
        SaveLocalProgress();
        
        // Reset earned coins counter since we've saved them
        totalCoinsEarned = 0;
        
        Debug.Log("[ScoreCoinManager] Data saved successfully");
    }
    
    private void SaveLocalProgress()
    {
        int existingCoins = PlayerPrefs.GetInt("PlayerCoins", 80);
        int updatedCoins = existingCoins + totalCoinsEarned;
        
        PlayerPrefs.SetInt("PlayerCoins", updatedCoins);
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        PlayerPrefs.Save();
        
        Debug.Log($"[ScoreCoinManager] Local progress saved - UpdatedCoins: {updatedCoins}, HighScore: {PlayerPrefs.GetInt("HighScore", 0)}");
        
        // Reset earned coins counter since we've saved them locally
        totalCoinsEarned = 0;
        Debug.Log("[ScoreCoinManager] TotalCoinsEarned reset to 0 after local save");
    }
    
    public void SaveProgress()
    {
        Debug.Log("[ScoreCoinManager] SaveProgress called manually");
        SaveProgressToDatabase();
    }
    
    public void OnGameEnd()
    {
        Debug.Log("[ScoreCoinManager] OnGameEnd called");
        SaveProgressToDatabase();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
    
    void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = " Coins: " + coins.ToString();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveProgressToDatabase();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveProgressToDatabase();
        }
    }
    
    void OnDestroy()
    {
        SaveProgressToDatabase();
    }
}
