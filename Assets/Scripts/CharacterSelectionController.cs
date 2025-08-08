using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionController : MonoBehaviour
{
    [Header("Character System")]
    public CharacterManager characterManager;
    
    [Header("UI Elements")]
    public TextMeshProUGUI coinText;
    public Button purchaseButton;
    public TextMeshProUGUI purchaseButtonText;
    public GameObject lockOverlay;
    public Button continueButton; 
    
    [Header("Visual Effects")]
    public Color lockedTint = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color unlockedTint = Color.white;
    
    private int currentCoins = 0;
    private bool isLoadingCoins = false; // Add this flag to prevent recursive calls
    private bool hasInitializedCoins = false; // Only initialize once
    
    void Start()
    {
        if (CharacterUnlockManager.Instance != null && characterManager != null)
        {
            CharacterUnlockManager.Instance.SetCharacterCollection(characterManager.charCollection);
        }
        
        InitializeUI();
        UpdateDisplay();
    }
    
    void InitializeUI()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(PurchaseCharacter);
        
        LoadCoins();
        
        if (!gameObject.CompareTag("UI"))
        {
            gameObject.tag = "UI";
        }
    }
    
    public void OnCharacterSelectionChanged(int characterIndex)
    {
        UpdateDisplay();
    }
    
    void LoadCoins()
    {
        if (isLoadingCoins || hasInitializedCoins)
        {
            return;
        }
        
        isLoadingCoins = true;
        
        // Try to use DatabaseManager singleton first
        DatabaseManager databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
        }
        
        if (databaseManager != null)
        {
            databaseManager.GetCoinsWithFallback((coins, source) => {
                currentCoins = coins;
                UpdateCoinDisplayOnly(); 
                isLoadingCoins = false;
                hasInitializedCoins = true;
            });
            return;
        }
        
        // Fallback to ScoreCoinManager if DatabaseManager not found
        ScoreCoinManager scoreManager = FindObjectOfType<ScoreCoinManager>();
        if (scoreManager != null)
        {
            currentCoins = scoreManager.GetCurrentCoins();
            Debug.Log($"Coins initialized from ScoreManager: {currentCoins}");
        }
        else
        {
            currentCoins = PlayerPrefs.GetInt("PlayerCoins", 100); 
            Debug.Log($"Coins initialized from PlayerPrefs: {currentCoins}");
        }
        
        UpdateCoinDisplayOnly();
        isLoadingCoins = false;
        hasInitializedCoins = true;
    }
    
    void UpdateDisplay()
    {
        if (characterManager == null || characterManager.charCollection == null)
            return;
            
        int selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
        
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterManager.charCollection.countCharacter)
            return;
            
        Character character = characterManager.charCollection.GetCharacter(selectedCharacterIndex);
        bool isUnlocked = IsCharacterUnlocked(selectedCharacterIndex);
        
        UpdateVisualState(isUnlocked);
        UpdatePurchaseButton(character, isUnlocked);
        UpdateContinueButton(isUnlocked); 
        UpdateCoinDisplay();
    }
    
    void UpdateVisualState(bool isUnlocked)
    {
        if (characterManager != null && characterManager.spriteRendered != null)
        {
            Color targetColor = isUnlocked ? unlockedTint : lockedTint;
            characterManager.spriteRendered.color = targetColor;
        }
        
        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!isUnlocked);
        }
    }
    void UpdatePurchaseButton(Character character, bool isUnlocked)
    {
        if (purchaseButton == null)
            return;
    
        if (!isUnlocked && character.unlockCost > 0)
        {
            purchaseButton.gameObject.SetActive(true);
            
            bool canAfford = currentCoins >= character.unlockCost;
            purchaseButton.interactable = canAfford;
            
            if (purchaseButtonText != null)
            {
                purchaseButtonText.text = $"Unlock for {character.unlockCost} coins";
                purchaseButtonText.color = canAfford ? Color.white : Color.gray;
            }
        }
        else
        {
            purchaseButton.gameObject.SetActive(false);
        }
    }
    
    void UpdateCoinDisplay()
    {
        if (!isLoadingCoins)
        {
            LoadCoins(); 
        }
        
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }
    
    void UpdateCoinDisplayOnly()
    {
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }
    
    void PurchaseCharacter()
    {
        if (characterManager == null || characterManager.charCollection == null)
            return;
            
        int selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
            
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterManager.charCollection.countCharacter)
            return;
            
        Character character = characterManager.charCollection.GetCharacter(selectedCharacterIndex);
        
        bool canUnlock = false;
        if (CharacterUnlockManager.Instance != null)
        {
            canUnlock = CharacterUnlockManager.Instance.CanUnlockCharacter(selectedCharacterIndex, currentCoins);
        }
        else
        {
            canUnlock = currentCoins >= character.unlockCost;
        }
        
        if (canUnlock)
        {
            bool unlocked = false;
            if (CharacterUnlockManager.Instance != null)
            {
                unlocked = CharacterUnlockManager.Instance.UnlockCharacter(selectedCharacterIndex, currentCoins);
            }
            
            if (unlocked || CharacterUnlockManager.Instance == null)
            {
                currentCoins -= character.unlockCost;
                SaveCoins();
                if (CharacterUnlockManager.Instance == null)
                {
                    PlayerPrefs.SetInt($"Character_{selectedCharacterIndex}_Unlocked", 1);
                    PlayerPrefs.Save();
                }
                
                UpdateDisplay();
                
                Debug.Log($"Unlocked {character.characterName} for {character.unlockCost} coins!");
            }
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }
    
    bool IsCharacterUnlocked(int characterIndex)
    {
        if (characterManager == null || characterManager.charCollection == null || characterIndex < 0 || characterIndex >= characterManager.charCollection.countCharacter)
            return false;
        
        if (CharacterUnlockManager.Instance != null)
        {
            return CharacterUnlockManager.Instance.IsCharacterUnlocked(characterIndex);
        }
        
        // Fallback logic
        Character character = characterManager.charCollection.GetCharacter(characterIndex);
        if (character.isUnlockedByDefault)
            return true;
        
        return PlayerPrefs.GetInt($"Character_{characterIndex}_Unlocked", 0) == 1;
    }
    
    void SaveCoins()
    {
        PlayerPrefs.SetInt("PlayerCoins", currentCoins);
        PlayerPrefs.Save();
        
        DatabaseManager databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
        }
        
        if (databaseManager != null)
        {
            databaseManager.SaveCoinsOnly(currentCoins, (success) => {
                if (!success)
                {
                    Debug.LogWarning("Failed to save coins via DatabaseManager, local save completed");
                }
            });
        }
        else
        {
            // Also update ScoreCoinManager if available as fallback
            ScoreCoinManager scoreManager = FindObjectOfType<ScoreCoinManager>();
            if (scoreManager != null)
            {
                scoreManager.SetCoins(currentCoins);
            }
        }
    }
    
    public bool CanPlayWithSelectedCharacter()
    {
        if (characterManager == null)
            return false;
            
        int selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
        return IsCharacterUnlocked(selectedCharacterIndex);
    }
    
    public void RefreshCoinsDisplay()
    {
        hasInitializedCoins = false;
        LoadCoins();
    }
    
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
    
    public void ForceReloadCoins()
    {
        hasInitializedCoins = false;
        LoadCoins();
    }
    
    void UpdateContinueButton(bool canPlay)
    {
        if (continueButton != null)
        {
            continueButton.interactable = canPlay;
            
            // Optional: Change button appearance when disabled
            if (continueButton.GetComponent<Image>() != null)
            {
                Color buttonColor = canPlay ? Color.white : Color.gray;
                continueButton.GetComponent<Image>().color = buttonColor;
            }
            
            // Optional: Update button text
            TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = canPlay ? "PLAY" : "LOCKED";
                buttonText.color = canPlay ? Color.white : Color.red;
            }
        }
    }
    
    // Debug method to test character unlock system
    [System.Obsolete("Debug method - remove in production")]
    public void DebugCharacterStates()
    {
        if (characterManager == null || characterManager.charCollection == null)
        {
            Debug.LogError("Cannot debug - CharacterManager or collection is null");
            return;
        }
        
        Debug.Log($"=== CHARACTER DEBUG INFO ===");
        Debug.Log($"CharacterUnlockManager.Instance: {CharacterUnlockManager.Instance != null}");
        Debug.Log($"Total characters: {characterManager.charCollection.countCharacter}");
        Debug.Log($"Current coins: {currentCoins}");
        Debug.Log($"Lock overlay reference: {(lockOverlay != null ? lockOverlay.name : "NULL")}");
        Debug.Log($"Purchase button reference: {(purchaseButton != null ? purchaseButton.name : "NULL")}");
        
        for (int i = 0; i < characterManager.charCollection.countCharacter; i++)
        {
            Character character = characterManager.charCollection.GetCharacter(i);
            bool unlocked = IsCharacterUnlocked(i);
            Debug.Log($"Character {i}: {character.characterName} - Cost: {character.unlockCost}, Default: {character.isUnlockedByDefault}, Unlocked: {unlocked}");
        }
        Debug.Log($"=== END DEBUG INFO ===");
    }
}
