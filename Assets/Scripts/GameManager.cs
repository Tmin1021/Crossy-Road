using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isGameOver = false;
    
    [Header("UI References")]
    public GameObject gameOverPanel;  // Assign in inspector if you have a game over UI panel
    public GameObject pausePanel;     // Assign in inspector if you have a pause UI panel
    
    private float originalTimeScale = 1f;
    
    void Awake()
    {
        // Singleton pattern
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
        ResumeGame(); // Ensure game starts unpaused
    }
    
    void Update()
    {
        // Optional: Add pause toggle with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }
        
        // Optional: Restart game with R key when game over
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // Optional: Return to main menu with M key when game over
        if (isGameOver && Input.GetKeyDown(KeyCode.M))
        {
            ReturnToMainMenu();
        }
    }
    
    public void PauseGame()
    {
        if (isGameOver) return; // Don't pause if game is already over
        
        isGamePaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (isGameOver) return; // Don't resume if game is over
        
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
        if (isGameOver) return; // Already game over
        
        isGameOver = true;
        isGamePaused = true;
        Time.timeScale = 0f;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        Debug.Log("Game Over - Game Paused");
        Debug.Log("Press R to restart or M to return to main menu");
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
        Time.timeScale = originalTimeScale;
        isGamePaused = false;
        isGameOver = false;
        
        // Load main menu scene (adjust scene name as needed)
        SceneManager.LoadScene("MainMenu");
        
        Debug.Log("Returning to Main Menu");
    }
    
    // Public method for external scripts to check if game is paused
    public bool IsGamePaused()
    {
        return isGamePaused || isGameOver;
    }
    
    // Public method for external scripts to check if game is over
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    void OnDestroy()
    {
        // Reset time scale when GameManager is destroyed
        if (Instance == this)
        {
            Time.timeScale = 1f;
        }
    }
}
