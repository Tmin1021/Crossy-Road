using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isGameOver = false;
    
    [Header("UI References")]
    public GameObject gameOverPanel;  
    public GameObject pausePanel;     
    
    [Header("Save System")]
    public float autoSaveInterval = 30f; 
    private float lastSaveTime;
    private bool isSavingAndExiting = false; 
    private float originalTimeScale = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        originalTimeScale = Time.timeScale;
        ResumeGame(); 
    }
    
    void Update()
    {
        if (!IsGamePaused() && !isGameOver)
        {
            if (Time.time - lastSaveTime > autoSaveInterval)
            {
                SaveGame();
                lastSaveTime = Time.time;
            }
        }
        
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    
        if (isGameOver && Input.GetKeyDown(KeyCode.M))
        {
            ReturnToMainMenu();
        }
    }
    
    public void PauseGame()
    {
        if (isGameOver) return; 
        
        isGamePaused = true;
        Time.timeScale = 0f;
        
        // Find pause panel dynamically if not assigned
        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel");
            if (pausePanel == null)
            {
                pausePanel = GameObject.Find("Pause Panel");
            }
        }
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (isGameOver || isSavingAndExiting) return; 
        
        isGamePaused = false;
        Time.timeScale = originalTimeScale;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        Debug.Log("Game Resumed");
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        isGamePaused = true;
        Time.timeScale = 0f;
        
        if (SaveSystemManager.Instance != null)
        {
            SaveSystemManager.Instance.ClearSaveData();
        }
        if (gameOverPanel == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>(true); 
            foreach (Canvas canvas in canvases)
            {
                Transform found = canvas.transform.Find("GameOverPanel");
                if (found != null)
                {
                    gameOverPanel = found.gameObject;
                    break;
                }
                
                found = FindChildRecursive(canvas.transform, "GameOverPanel");
                if (found != null)
                {
                    gameOverPanel = found.gameObject;
                    break;
                }
            }
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        Debug.Log("Game Over");
    }
    
    public void SaveGame()
    {
        if (SaveSystemManager.Instance != null)
        {
            SaveSystemManager.Instance.SaveGameState();
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = originalTimeScale;
        isGamePaused = false;
        isGameOver = false;
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Debug.Log("Game Restarted");
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("ReturnToMainMenu called");
        Time.timeScale = originalTimeScale;
        isGamePaused = false;
        isGameOver = false; 
        if (!isGameOver)
        {
            SaveGame();
        }
        
        Debug.Log("About to load MainMenu scene");
        try
        {
            SceneManager.LoadScene("MainMenu");
            Debug.Log("Scene load initiated");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load MainMenu scene by name: {e.Message}");
            
            try
            {
                Debug.Log("Trying to load scene by index 0");
                SceneManager.LoadScene(0);
            }
            catch (System.Exception e2)
            {
                Debug.LogError($"Failed to load scene by index: {e2.Message}");
            }
        }
        
        Debug.Log("Returning to Main Menu");
    }
    
    public void SaveAndExit()
    {
        if (isSavingAndExiting) return;
        
        Debug.Log("Save and Exit to Main Menu");
        isSavingAndExiting = true;
        
        // Save immediately without coroutine
        if (SaveSystemManager.Instance != null)
        {
            SaveSystemManager.Instance.SaveGameState();
            Debug.Log("Save completed, now returning to main menu");
        }
        
        // Reset flag and go to main menu immediately
        isSavingAndExiting = false;
        ReturnToMainMenu();
    }
    
    public bool IsGamePaused()
    {
        return isGamePaused || isGameOver;
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
        }
    }
    
    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            
            Transform found = FindChildRecursive(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
}
