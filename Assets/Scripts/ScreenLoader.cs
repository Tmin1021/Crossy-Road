using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad) && SceneManager.GetSceneByName(sceneToLoad) != null)
        {
            BackButton.SetPreviousScene(SceneManager.GetActiveScene().name); 
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene '" + sceneToLoad + "' not found in Build Settings!");
        }
    }
}
