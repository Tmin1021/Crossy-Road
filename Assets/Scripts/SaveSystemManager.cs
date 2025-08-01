using UnityEngine;
using System.IO;

public class SaveSystemManager : MonoBehaviour
{
    public static SaveSystemManager Instance { get; private set; }
    
    private string saveFilePath;
    private const string SAVE_FILE_NAME = "gamesave.json";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SetupSaveLocation();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void SetupSaveLocation()
    {
        // Use Assets/Files folder directly
        string assetsFilesFolder = Path.Combine(Application.dataPath, "Files");
        saveFilePath = Path.Combine(assetsFilesFolder, SAVE_FILE_NAME);
        Debug.Log($"✓ Using save location: {saveFilePath}");
    }
    
    public void SaveGameState()
    {
        Debug.Log("=== Starting SaveGameState ===");
        
        GameSaveData saveData = new GameSaveData();
        
        saveData.gameMode = PlayerPrefs.GetInt("SelectedGameMode", 1);
        saveData.isTwoPlayerMode = PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1;
        saveData.player1CharacterIndex = PlayerPrefs.GetInt("Player1Character", 0);
        saveData.player2CharacterIndex = PlayerPrefs.GetInt("Player2Character", 0);
        
        GameObject player1 = GameObject.Find("Player1");
        GameObject player2 = GameObject.Find("Player2");
        
        if (player1 != null)
        {
            saveData.player1Position = player1.transform.position;
            PlayerMovement pm1 = player1.GetComponent<PlayerMovement>();
            if (pm1 != null)
            {
                saveData.lastLaneY = pm1.transform.position.y; // Save progress
            }
        }
        
        if (player2 != null)
        {
            saveData.player2Position = player2.transform.position;
        }
        
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            saveData.currentScore = scoreManager.GetCurrentScore();
        }
        
        if (Camera.main != null)
        {
            saveData.cameraPosition = Camera.main.transform.position;
        }
        
        LaneManager laneManager = FindObjectOfType<LaneManager>();
        if (laneManager != null)
        {
            saveData.lastSpawnY = laneManager.lastSpawnY;
        }
        
        saveData.volume = AudioListener.volume;
        
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            Debug.Log($"Saving to: {saveFilePath}");
            File.WriteAllText(saveFilePath, json);
            Debug.Log("File write completed");
            
            // Mark game as in progress
            PlayerPrefs.SetInt("GameInProgress", 1);
            PlayerPrefs.SetString("LastSaveTime", saveData.saveTimestamp);
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs saved");
            
            // Verify the save immediately
            bool fileExists = File.Exists(saveFilePath);
            Debug.Log($"Immediate verification - File exists: {fileExists}");
            
            Debug.Log("Game state saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game state: " + e.Message);
        }
    }
    
    public GameSaveData LoadGameState()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log("Game state loaded successfully!");
                return saveData;
            }
            else
            {
                Debug.Log("No save file found.");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game state: " + e.Message);
            return null;
        }
    }
    
    public void ClearSaveData()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
            
            PlayerPrefs.SetInt("GameInProgress", 0);
            PlayerPrefs.DeleteKey("LastSaveTime");
            PlayerPrefs.Save();
            
            Debug.Log("Save data cleared.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to clear save data: " + e.Message);
        }
    }
    
    public bool HasSaveData()
    {
        bool fileExists = File.Exists(saveFilePath);
        bool playerPrefSet = PlayerPrefs.GetInt("GameInProgress", 0) == 1;
        
        Debug.Log($"HasSaveData check - File exists: {fileExists}, PlayerPref set: {playerPrefSet}");
        Debug.Log($"Save file path: {saveFilePath}");
        
        return fileExists && playerPrefSet;
    }
}
