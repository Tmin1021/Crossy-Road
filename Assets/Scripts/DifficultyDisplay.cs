using UnityEngine;
using TMPro;

public class DifficultyDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI difficultyText;
    
    [Header("Display Settings")]
    public bool showMultipliers = true;
    
    void Start()
    {
        UpdateDisplay();
        
        // Listen for difficulty changes
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.OnDifficultyChanged += OnDifficultyChanged;
        }
    }
    
    void OnDestroy()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.OnDifficultyChanged -= OnDifficultyChanged;
        }
    }
    
    void OnDifficultyChanged(GameDifficulty newDifficulty)
    {
        UpdateDisplay();
    }
    
    void UpdateDisplay()
    {
        if (difficultyText == null || GameModeManager.Instance == null) return;
        
        string displayText = GameModeManager.Instance.GetDifficultyName().ToUpper();
        
        if (showMultipliers)
        {
            float speedMult = GameModeManager.Instance.GetVehicleSpeedMultiplier();
            int scoreMult = GameModeManager.Instance.GetScoreMultiplier();
            
            displayText += $"\nSpeed: {speedMult:F1}x";
            if (scoreMult > 1)
            {
                displayText += $" | Score: {scoreMult}x";
            }
        }
        
        difficultyText.text = displayText;
        difficultyText.color = GameModeManager.Instance.GetDifficultyColor();
    }
}
