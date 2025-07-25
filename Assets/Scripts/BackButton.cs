using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    private static string previousScene;

    // Call this function to load the previous scene
    public void LoadPreviousScene()
    {
        // Check if there's a valid previous scene stored
        if (!string.IsNullOrEmpty(previousScene) && SceneManager.GetSceneByName(previousScene) != null)
        {
            // Load the previous scene
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogError("Previous scene not found or not set.");
        }
    }

    // Call this function whenever you want to update the previous scene when changing scenes
    public static void SetPreviousScene(string currentScene)
    {
        previousScene = currentScene;
    }
}
