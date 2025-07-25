using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad; // Assign the target scene name in the Inspector

    public void LoadScene()
{
    if (!string.IsNullOrEmpty(sceneToLoad) && SceneManager.GetSceneByName(sceneToLoad) != null)
    {
        SceneManager.LoadScene(sceneToLoad); // Single mode is default
    }
    else
    {
        Debug.LogError("Scene '" + sceneToLoad + "' not found in Build Settings!");
    }
}

}