using UnityEngine;
using System.Collections.Generic;

public class CharacterUnlockManager : MonoBehaviour
{
    public static CharacterUnlockManager Instance;
    
    [Header("Character Collection")]
    public CharacterCollection characterCollection;
    
    private HashSet<int> unlockedCharacters;
    private const string UNLOCKED_CHARACTERS_KEY = "UnlockedCharacters";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUnlockedCharacters();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadUnlockedCharacters()
    {
        unlockedCharacters = new HashSet<int>();
        
        string unlockedData = PlayerPrefs.GetString(UNLOCKED_CHARACTERS_KEY, "");
        
        // Check for corruption: if the data contains too many characters, it's likely corrupted
        if (!string.IsNullOrEmpty(unlockedData))
        {
            string[] unlockedIndices = unlockedData.Split(',');
            
            // If more than 2 characters are "manually unlocked", it's probably corrupted from the old system
            if (unlockedIndices.Length > 2)
            {
                PlayerPrefs.DeleteKey(UNLOCKED_CHARACTERS_KEY);
                PlayerPrefs.Save();
            }
            else
            {
                // Load the clean data
                foreach (string index in unlockedIndices)
                {
                    if (int.TryParse(index, out int charIndex))
                    {
                        unlockedCharacters.Add(charIndex);
                    }
                }
            }
        }
        
        SaveUnlockedCharacters();
    }
    
    void SaveUnlockedCharacters()
    {
        List<string> unlockedList = new List<string>();
        foreach (int index in unlockedCharacters)
        {
            unlockedList.Add(index.ToString());
        }
        
        string unlockedData = string.Join(",", unlockedList.ToArray());
        PlayerPrefs.SetString(UNLOCKED_CHARACTERS_KEY, unlockedData);
        PlayerPrefs.Save();
    }
    
    public bool IsCharacterUnlocked(int characterIndex)
    {
        if (characterCollection == null || characterIndex < 0 || characterIndex >= characterCollection.countCharacter)
        {
            Debug.LogWarning($"CharacterUnlockManager: Invalid parameters - collection null: {characterCollection == null}, index: {characterIndex}");
            return false;
        }
        
        Character character = characterCollection.GetCharacter(characterIndex);
        
        if (character.isUnlockedByDefault)
        {
            Debug.Log($"CharacterUnlockManager: Character {characterIndex} ({character.characterName}) unlocked by default");
            return true;
        }
        
        bool unlocked = unlockedCharacters.Contains(characterIndex);
        Debug.Log($"CharacterUnlockManager: Character {characterIndex} ({character.characterName}) purchased unlock status: {unlocked}");
        return unlocked;
    }
    
    public bool CanUnlockCharacter(int characterIndex, int playerCoins)
    {
        if (IsCharacterUnlocked(characterIndex))
            return false;
            
        if (characterCollection == null || characterIndex < 0 || characterIndex >= characterCollection.countCharacter)
            return false;
            
        Character character = characterCollection.GetCharacter(characterIndex);
        return playerCoins >= character.unlockCost;
    }
    
    public bool UnlockCharacter(int characterIndex, int playerCoins)
    {
        if (!CanUnlockCharacter(characterIndex, playerCoins))
            return false;
            
        Character character = characterCollection.GetCharacter(characterIndex);
        
        unlockedCharacters.Add(characterIndex);
        SaveUnlockedCharacters();
        
        Debug.Log($"Unlocked character: {character.characterName}");
        return true;
    }
    
    public int GetCharacterCost(int characterIndex)
    {
        if (characterCollection == null || characterIndex < 0 || characterIndex >= characterCollection.countCharacter)
            return 0;
            
        Character character = characterCollection.GetCharacter(characterIndex);
        return character.unlockCost;
    }
    
    public string GetCharacterName(int characterIndex)
    {
        if (characterCollection == null || characterIndex < 0 || characterIndex >= characterCollection.countCharacter)
            return "Unknown";
            
        Character character = characterCollection.GetCharacter(characterIndex);
        return character.characterName;
    }
    
    public void SetCharacterCollection(CharacterCollection collection)
    {
        if (characterCollection != collection)
        {
            characterCollection = collection;
            Debug.Log($"CharacterUnlockManager: Character collection set to {(collection != null ? collection.name : "null")}");
            if (collection != null)
            {
                Debug.LogWarning("CharacterUnlockManager: Clearing PlayerPrefs to fix any corruption from mixed default/manual unlock data");
                PlayerPrefs.DeleteKey(UNLOCKED_CHARACTERS_KEY);
                PlayerPrefs.Save();
                
                LoadUnlockedCharacters(); 
            }
        }
    }
    
    public void ResetAllUnlocks()
    {
        PlayerPrefs.DeleteKey(UNLOCKED_CHARACTERS_KEY);
        unlockedCharacters.Clear();
        LoadUnlockedCharacters();
        Debug.Log("All character unlocks reset!");
    }
    
    public void ForceRefresh()
    {
        Debug.Log("CharacterUnlockManager: Force refreshing unlock data...");
        LoadUnlockedCharacters();
    }
}
