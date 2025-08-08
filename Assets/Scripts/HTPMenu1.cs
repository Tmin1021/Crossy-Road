using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HTPMenu1 : MonoBehaviour
{
    [Header("UI References")]
    public Button nextScreenButton;

    public Button returnButton;

    [Header("Scene Settings")]
    public string htpSceneName = "HowToPlay2";
    public string mainMenu = "MainMenu";

    void Start()
    {
        nextScreenButton.onClick.AddListener(MoveToHtp2);
        returnButton.onClick.AddListener(returnToMain);
    }

    public void MoveToHtp2()
    {
        if (!string.IsNullOrEmpty(htpSceneName))
        {
            SceneManager.LoadScene(htpSceneName);
        }
        else
        {
            Debug.LogWarning("Scoreboard scene name not set!");
        }
    }
    public void returnToMain()
    {
        if (!string.IsNullOrEmpty(mainMenu))
        {
            SceneManager.LoadScene(mainMenu);
        }
        else
        {
            Debug.LogWarning("Scoreboard scene name not set!");
        }
    }
}
