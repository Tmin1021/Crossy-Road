using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultySelector : MonoBehaviour
{
    [Header("UI References")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    
    [Header("Button Colors")]
    public Color selectedColor = Color.white;
    public Color unselectedColor = Color.gray;
    
    [Header("Difficulty Info")]
    public TextMeshProUGUI difficultyInfoText;
    
    private GameDifficulty selectedDifficulty = GameDifficulty.Medium;
    
    void Start()
    {
        SetupButtons();
        LoadSelectedDifficulty();
        UpdateUI();
    }
    
    void SetupButtons()
    {
        if (easyButton != null)
            easyButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Easy));
        
        if (mediumButton != null)
            mediumButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Medium));
        
        if (hardButton != null)
            hardButton.onClick.AddListener(() => SelectDifficulty(GameDifficulty.Hard));
    }
    
    public void SelectDifficulty(GameDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
        SaveSelectedDifficulty();
        UpdateUI();
        Debug.Log($"Difficulty selected: {difficulty}");
    }
    
    void UpdateUI()
    {
        UpdateButtonColor(easyButton, selectedDifficulty == GameDifficulty.Easy);
        UpdateButtonColor(mediumButton, selectedDifficulty == GameDifficulty.Medium);
        UpdateButtonColor(hardButton, selectedDifficulty == GameDifficulty.Hard);
        UpdateDifficultyInfo();
    }
    
    void UpdateButtonColor(Button button, bool isSelected)
    {
        if (button == null) return;
        
        Color targetColor = isSelected ? selectedColor : unselectedColor;
        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = targetColor;
    }
    
    void UpdateDifficultyInfo()
    {
        if (difficultyInfoText == null) return;
        
        string infoText = "";
        switch (selectedDifficulty)
        {
            case GameDifficulty.Easy:
                infoText = "EASY MODE\n• Slower vehicles\n• More time between spawns\n• Normal score";
                break;
            case GameDifficulty.Medium:
                infoText = "MEDIUM MODE\n• Normal vehicle speed\n• Normal spawn rate\n• Normal score";
                break;
            case GameDifficulty.Hard:
                infoText = "HARD MODE\n• Faster vehicles\n• More frequent spawns\n• Double score!";
                break;
        }
        
        difficultyInfoText.text = infoText;
        difficultyInfoText.color = GetDifficultyColor();
    }
    
    Color GetDifficultyColor()
    {
        switch (selectedDifficulty)
        {
            case GameDifficulty.Easy: return Color.green;
            case GameDifficulty.Medium: return Color.yellow;
            case GameDifficulty.Hard: return Color.red;
            default: return Color.white;
        }
    }
    
    void SaveSelectedDifficulty()
    {
        PlayerPrefs.SetInt("GameDifficulty", (int)selectedDifficulty);
        PlayerPrefs.Save();
    }
    
    void LoadSelectedDifficulty()
    {
        int savedDifficulty = PlayerPrefs.GetInt("GameDifficulty", (int)GameDifficulty.Medium);
        selectedDifficulty = (GameDifficulty)savedDifficulty;
    }
    
    public GameDifficulty GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
}
