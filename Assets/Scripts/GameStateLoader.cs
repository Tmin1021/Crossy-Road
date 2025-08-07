using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStateLoader : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("LoadSavedGame", 0) == 1)
        {
            StartCoroutine(LoadSavedGameState());
            PlayerPrefs.SetInt("LoadSavedGame", 0);
        }
    }
    
    IEnumerator LoadSavedGameState()
    {
        yield return new WaitForEndOfFrame();
        
        if (SaveSystemManager.Instance != null)
        {
            GameSaveData saveData = SaveSystemManager.Instance.LoadGameState();
            if (saveData != null)
            {
                ApplySaveData(saveData);
            }
        }
    }
    
    void ApplySaveData(GameSaveData saveData)
    {
        PlayerPrefs.SetInt("SelectedGameMode", saveData.gameMode);
        PlayerPrefs.SetInt("IsTwoPlayerMode", saveData.isTwoPlayerMode ? 1 : 0);
        PlayerPrefs.SetInt("Player1Character", saveData.player1CharacterIndex);
        PlayerPrefs.SetInt("Player2Character", saveData.player2CharacterIndex);
        
        StartCoroutine(RestorePlayerPositions(saveData));
        
        if (Camera.main != null)
        {
            Camera.main.transform.position = saveData.cameraPosition;
        }
        
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
        }
        
        AudioListener.volume = saveData.volume;
        
        Debug.Log("Game state restored!");
    }
    
    IEnumerator RestorePlayerPositions(GameSaveData saveData)
    {
        yield return new WaitForSeconds(0.5f); 
        
        GameObject player1 = GameObject.Find("Player1");
        GameObject player2 = GameObject.Find("Player2");
        
        if (player1 != null)
        {
            player1.transform.position = saveData.player1Position;
        }
        
        if (player2 != null && saveData.isTwoPlayerMode)
        {
            player2.transform.position = saveData.player2Position;
        }
    }
}
