using UnityEngine;

public class ContinueButtonDebugger : MonoBehaviour
{
    void Start()
    {
        if (SaveSystemManager.Instance != null)
        {
            bool hasSave = SaveSystemManager.Instance.HasSaveData();
        }
        else
        {
            Debug.LogError("✗ SaveSystemManager.Instance is NULL!");
        }
        
        int gameInProgress = PlayerPrefs.GetInt("GameInProgress", 0);
        int loadSavedGame = PlayerPrefs.GetInt("LoadSavedGame", 0);
        
        GameStateLoader loader = FindObjectOfType<GameStateLoader>();
        if (loader != null)
        {
            Debug.Log("✓ GameStateLoader found in scene");
        }
        else
        {
            Debug.LogError("✗ GameStateLoader NOT found in scene!");
        }
    }
}
