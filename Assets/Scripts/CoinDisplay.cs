using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI coinText;
    public Text legacyCoinText;
    
    private int currentCoins = 0;
    
    void Start()
    {
        UpdateCoinDisplay();
        InvokeRepeating(nameof(UpdateCoinDisplay), 1f, 1f);
    }
    
    public void UpdateCoinDisplay()
    {
        // Get coins from multiple sources
        currentCoins = GetPlayerCoins();
        
        // Update UI
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
        else if (legacyCoinText != null)
        {
            legacyCoinText.text = currentCoins.ToString();
        }
    }
    
    int GetPlayerCoins()
    {
        // Try to get from ScoreCoinManager first
        ScoreCoinManager scoreManager = FindObjectOfType<ScoreCoinManager>();
        if (scoreManager != null)
        {
            return scoreManager.GetCurrentCoins();
        }
        
        // Fallback to PlayerPrefs
        return PlayerPrefs.GetInt("PlayerCoins", 0);
    }
    
    public int GetCurrentCoins()
    {
        return currentCoins;
    }
    
    // Method to manually refresh (call this when coins change)
    public void RefreshCoins()
    {
        UpdateCoinDisplay();
    }
}
