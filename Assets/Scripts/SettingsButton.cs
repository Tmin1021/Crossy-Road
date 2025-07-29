using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public SettingsManager settingsManager;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null && settingsManager != null)
        {
            button.onClick.AddListener(OpenSettings);
        }
    }

    void OpenSettings()
    {
        if (settingsManager != null)
        {
            settingsManager.OpenSettings();
        }
    }
}
