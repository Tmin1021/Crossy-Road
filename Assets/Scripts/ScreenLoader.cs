using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad; // Assign the target scene name in the Inspector

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad) && SceneManager.GetSceneByName(sceneToLoad) != null)
        {
            // Update the previous scene before loading the new scene
            BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);  // Set the current scene as the previous scene
            
            // Load the new scene (sceneToLoad)
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene '" + sceneToLoad + "' not found in Build Settings!");
        }
    }
}
