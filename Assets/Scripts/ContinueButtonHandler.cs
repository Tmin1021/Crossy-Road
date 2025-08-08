using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ContinueButtonHandler : MonoBehaviour
{
    [Header("UI References")]
    public Button continueButton;
    public CharacterManager characterManager;
    
    [Header("Scene Settings")]
    public string gameSceneName = "Game";
    
    void Start()
    {
        InitializeButton();
        UpdateButtonState();
    }
    
    void OnEnable()
    {
        UpdateButtonState();
    }
    
    void InitializeButton()
    {
        if (continueButton == null)
            continueButton = GetComponent<Button>();
            
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }
    
    public void UpdateButtonState()
    {
        if (continueButton == null || characterManager == null)
            return;
            
        bool canPlay = CanPlayWithSelectedCharacter();
        continueButton.interactable = canPlay;
        
        // Optional: Update button appearance based on state
        var buttonImage = continueButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = canPlay ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
        }
    }
    
    bool CanPlayWithSelectedCharacter()
    {
        if (characterManager == null)
            return false;
            
        int selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
        
        // Check if character is unlocked through CharacterUnlockManager
        if (CharacterUnlockManager.Instance != null)
        {
            return CharacterUnlockManager.Instance.IsCharacterUnlocked(selectedCharacterIndex);
        }
        
        // Fallback: check PlayerPrefs directly
        return PlayerPrefs.GetInt($"Character_{selectedCharacterIndex}_Unlocked", 0) == 1;
    }
    
    void OnContinueClicked()
    {
        if (!CanPlayWithSelectedCharacter())
            return;
            
        // Save the selected character before starting the game
        if (characterManager != null)
        {
            int selectedIndex = characterManager.GetSelectedCharacterIndex();
            PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
            PlayerPrefs.Save();
        }
        
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Public method for external updates (called by CharacterSelectionController)
    public void RefreshButtonState()
    {
        UpdateButtonState();
    }
}
