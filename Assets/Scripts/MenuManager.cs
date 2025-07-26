using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button onePlayerButton;
    public Button twoPlayerButton;
    public Button nextScreenButton;
    public CharacterManager characterManager;
    
    [Header("Scene Settings")]
    public string playSceneName = "SampleScene";
    
    private int selectedMode = 1; // 1 = single player, 2 = two player
    private int selectedCharacterIndex = 0;

    void Start()
    {
        // Setup button listeners
        onePlayerButton.onClick.AddListener(() => SelectMode(1));
        twoPlayerButton.onClick.AddListener(() => SelectMode(2));
        nextScreenButton.onClick.AddListener(StartGame);
        
        // Initialize with default mode
        SelectMode(1);
    }

    public void SelectMode(int mode)
    {
        selectedMode = mode;
        
        // Update button visuals to show selection
        onePlayerButton.interactable = (mode != 1);
        twoPlayerButton.interactable = (mode != 2);
        
        Debug.Log($"Selected mode: {mode}P");
    }

    public void OnCharacterChanged(int characterIndex)
    {
        selectedCharacterIndex = characterIndex;
        Debug.Log($"Selected character index: {characterIndex}");
    }

    public void StartGame()
    {
        // Save selected mode
        PlayerPrefs.SetInt("SelectedGameMode", selectedMode);
        PlayerPrefs.SetInt("IsTwoPlayerMode", selectedMode == 2 ? 1 : 0);
        
        // Save selected character (get current selection from CharacterManager)
        if (characterManager != null)
        {
            selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
        }
        PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        
        // Save the selection for Player 1
        PlayerPrefs.SetInt("Player1Character", selectedCharacterIndex);
        // For 2P mode, you might want to add Player 2 selection later
        if (selectedMode == 2)
        {
            PlayerPrefs.SetInt("Player2Character", selectedCharacterIndex); // Same character for now
        }
        
        Debug.Log($"Starting game: Mode={selectedMode}P, Character={selectedCharacterIndex}");
        
        // Load the play scene
        SceneManager.LoadScene(playSceneName);
    }
}

//     void Start()
//     {
//         // Add click listeners to mode buttons
//         onePlayerButton.onClick.AddListener(() => SelectMode(false));
//         twoPlayerButton.onClick.AddListener(() => SelectMode(true));
//         startGameButton.onClick.AddListener(StartGame);

//         // Setup character buttons with sprites and listeners
//         SetupCharacterButtons(player1CharacterButtons, 1);
//         SetupCharacterButtons(player2CharacterButtons, 2);
//     }

//     void SetupCharacterButtons(Button[] buttons, int playerNum)
//     {
//         if (PlayerData.Instance == null)
//         {
//             Debug.LogError("PlayerData instance not found!");
//             return;
//         }

//         for (int i = 0; i < buttons.Length; i++)
//         {
//             if (i < PlayerData.Instance.availableCharacters.Length)
//             {
//                 int characterIndex = i;
//                 // Set the sprite image
//                 Image buttonImage = buttons[i].GetComponent<Image>();
//                 if (buttonImage != null)
//                 {
//                     buttonImage.sprite = PlayerData.Instance.availableCharacters[i].characterSprite;
//                     buttonImage.preserveAspect = true;
//                 }
//                 // Add click listener
//                 buttons[i].onClick.AddListener(() => SelectCharacter(playerNum, characterIndex));
//                 buttons[i].gameObject.SetActive(true);
//             }
//             else
//             {
//                 // Hide extra buttons if we have fewer characters than buttons
//                 buttons[i].gameObject.SetActive(false);
//             }
//         }

//         // Start with mode selection
//         ShowModeSelection();
//     }

//     void ShowModeSelection()
//     {
//         modeSelectionPanel.SetActive(true);
//         characterSelectionPanel.SetActive(false);
//     }

//     void SelectMode(bool isTwoPlayer)
//     {
//         isTwoPlayerMode = isTwoPlayer;
//         player2Selection.SetActive(isTwoPlayer);
        
//         // Switch to character selection
//         modeSelectionPanel.SetActive(false);
//         characterSelectionPanel.SetActive(true);
//     }

//     void SelectCharacter(int playerNum, int characterIndex)
//     {
//         PlayerData.Instance.SavePlayerSelection(playerNum, characterIndex);
        
//         // Highlight selected character button
//         Button[] buttons = playerNum == 1 ? player1CharacterButtons : player2CharacterButtons;
//         foreach (Button btn in buttons)
//         {
//             btn.interactable = true;
//         }
//         buttons[characterIndex].interactable = false;
//     }

//     void StartGame()
//     {
//         PlayerPrefs.SetInt("IsTwoPlayerMode", isTwoPlayerMode ? 1 : 0);
//         SceneManager.LoadScene(gameSceneName);
//     }
// }
