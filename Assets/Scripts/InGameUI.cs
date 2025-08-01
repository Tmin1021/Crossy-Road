using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("UI References")]
    public Button pauseButton;
    public Button saveAndExitButton;
    
    void Start()
    {
        SetupUI();
    }
    
    void SetupUI()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
            
        if (saveAndExitButton != null)
            saveAndExitButton.onClick.AddListener(SaveAndExit);
    }
    
    public void TogglePause()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGamePaused())
            {
                GameManager.Instance.ResumeGame();
                UpdatePauseButtonText("Pause");
            }
            else
            {
                GameManager.Instance.PauseGame();
                UpdatePauseButtonText("Resume");
            }
        }
    }
    
    public void SaveAndExit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveAndExit();
        }
    }
    
    void UpdatePauseButtonText(string text)
    {
        if (pauseButton != null)
        {
            Text buttonText = pauseButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }
    }
}
