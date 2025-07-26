using UnityEngine;

[System.Serializable]
public class Character
{
    public string characterName;
    public Sprite characterSprite;
    public RuntimeAnimatorController animatorController;
}

// public class PlayerData : MonoBehaviour
// {
//     public static PlayerData Instance;
//     public Character[] availableCharacters;
    
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     public void SavePlayerSelection(int playerIndex, int characterIndex)
//     {
//         PlayerPrefs.SetInt($"Player{playerIndex}Character", characterIndex);
//     }

//     public Character GetPlayerCharacter(int playerIndex)
//     {
//         int characterIndex = PlayerPrefs.GetInt($"Player{playerIndex}Character", 0);
//         return availableCharacters[characterIndex];
//     }
// }
