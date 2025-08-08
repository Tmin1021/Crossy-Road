using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HTPMenu2 : MonoBehaviour
{
    [Header("UI References")]

    public Button returnButton;

    [Header("Scene Settings")]
    public string htpSceneName = "HowToPlay1";

    void Start()
    {
        returnButton.onClick.AddListener(MoveToHtp1);
    }

    public void MoveToHtp1()
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
}
