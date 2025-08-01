using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button saveAndExitButton;
    public Button settingsButton;
    public Button restartButton;
    
    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    
    private bool isPauseMenuOpen = false;
    
    void Start()
    {
        SetupUI();
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    void SetupUI()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (saveAndExitButton != null)
            saveAndExitButton.onClick.AddListener(SaveAndExit);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
            {
                TogglePauseMenu();
            }
        }
    }
    
    public void TogglePauseMenu()
    {
        if (isPauseMenuOpen)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }
    
    public void OpenPauseMenu()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
        {
            isPauseMenuOpen = true;
            
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
            
            GameManager.Instance.PauseGame();
            Debug.Log("Pause menu opened");
        }
    }
    
    public void ClosePauseMenu()
    {
        isPauseMenuOpen = false;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        
        Debug.Log("Pause menu closed");
    }
    
    public void ResumeGame()
    {
        ClosePauseMenu();
    }
    
    public void SaveAndExit()
    {
        Debug.Log("Save and Exit clicked");
        
        isPauseMenuOpen = false;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveAndExit();
        }
    }
    
}
