using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    
    [Header("UI Settings")]
    public Color player1Color = Color.white;
    public Color player2Color = Color.white;
    
    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            InitializeScoreUI();
        }
        else
        {
            StartCoroutine(WaitForScoreManager());
        }
    }
    
    System.Collections.IEnumerator WaitForScoreManager()
    {
        while (ScoreManager.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        InitializeScoreUI();
    }
    
    void InitializeScoreUI()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
        
        bool isMultiplayer = IsMultiplayerMode();
        SetMultiplayerMode(isMultiplayer);
        
        UpdateAllScores();
        SetupColors();
        
        Debug.Log($"ScoreUI initialized. Multiplayer mode: {isMultiplayer}");
    }
    
    bool IsMultiplayerMode()
    {
        int gameMode = PlayerPrefs.GetInt("SelectedGameMode", 1);
        bool isTwoPlayer = PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1;
        
        return (gameMode == 2 || isTwoPlayer);
    }
    
    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
        }
    }
    
    void SetupColors()
    {
        if (player1ScoreText != null)
            player1ScoreText.color = player1Color;
        if (player2ScoreText != null)
            player2ScoreText.color = player2Color;
    }
    
    void UpdateScoreDisplay(int playerID, int newScore)
    {
        if (playerID == 1 && player1ScoreText != null)
        {
            player1ScoreText.text = $"P1: {newScore:N0}";
        }
        else if (playerID == 2 && player2ScoreText != null)
        {
            player2ScoreText.text = $"P2: {newScore:N0}";
            Debug.Log($"Updated P2 score display to: {newScore}");
        }
    }
    
    void UpdateAllScores()
    {
        if (ScoreManager.Instance == null) 
        {
            Debug.LogWarning("ScoreManager.Instance is null in UpdateAllScores");
            return;
        }
        
        UpdateScoreDisplay(1, ScoreManager.Instance.GetScore(1));
        UpdateScoreDisplay(2, ScoreManager.Instance.GetScore(2));
    }
    
    public void SetMultiplayerMode(bool isMultiplayer)
    {
        if (player2ScoreText != null)
        {
            player2ScoreText.gameObject.SetActive(isMultiplayer);
        }
        else
        {
            Debug.LogWarning("player2ScoreText is null!");
        }
        
        if (player1ScoreText != null)
        {
            player1ScoreText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("player1ScoreText is null!");
        }
    }
}