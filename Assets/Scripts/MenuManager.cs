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
    
    private int selectedMode = 1; 
    private int selectedCharacterIndex = 0;

    void Start()
    {
        onePlayerButton.onClick.AddListener(() => SelectMode(1));
        twoPlayerButton.onClick.AddListener(() => SelectMode(2));
        nextScreenButton.onClick.AddListener(StartGame);
        
        SelectMode(1);
    }

    public void SelectMode(int mode)
    {
        selectedMode = mode;
        
        onePlayerButton.interactable = (mode != 1);
        twoPlayerButton.interactable = (mode != 2);
    }

    public void OnCharacterChanged(int characterIndex)
    {
        selectedCharacterIndex = characterIndex;
        Debug.Log($"Selected character index: {characterIndex}");
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("SelectedGameMode", selectedMode);
        PlayerPrefs.SetInt("IsTwoPlayerMode", selectedMode == 2 ? 1 : 0);
        
        if (characterManager != null)
        {
            selectedCharacterIndex = characterManager.GetSelectedCharacterIndex();
        }
        PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        PlayerPrefs.SetInt("Player1Character", selectedCharacterIndex);

        if (selectedMode == 2)
        {
            PlayerPrefs.SetInt("Player2Character", selectedCharacterIndex); 
        }
    
        
        PlayerPrefs.Save();
        
        Debug.Log($"Starting game: Mode={selectedMode}P, Character={selectedCharacterIndex}");
        
        // Load the play scene
        SceneManager.LoadScene(playSceneName);
    }
}
