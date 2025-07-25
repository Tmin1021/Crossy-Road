using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Globalization;

public class KeyInputSaver : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] p1Inputs = new TMP_InputField[4];
    [SerializeField] private TMP_InputField[] p2Inputs = new TMP_InputField[4];
    [SerializeField] private Slider soundSlider, musicSlider;
    [SerializeField] private Button saveButton;

    private readonly string filePathP1 = Path.Combine(Application.streamingAssetsPath, "p1_key.txt");
    private readonly string filePathP2 = Path.Combine(Application.streamingAssetsPath, "p2_key.txt");
    private readonly string filePathSoundMusic = Path.Combine(Application.streamingAssetsPath, "sound_music.txt");

    private string[] initialP1Keys = new string[4];
    private string[] initialP2Keys = new string[4];
    private float initialSoundValue = -1f;
    private float initialMusicValue = -1f;

    private void Start()
    {
        LoadSettings();
        InitializeUI();
        SetupListeners();
    }

    private void LoadSettings()
    {
        LoadSliderValues();
        LoadKeyBindings();
    }

    private void LoadSliderValues()
    {
        if (File.Exists(filePathSoundMusic))
        {
            string[] lines = File.ReadAllLines(filePathSoundMusic);
            if (lines.Length >= 2)
            {
                initialSoundValue = ParseFloat(lines[0], 0f);
                initialMusicValue = ParseFloat(lines[1], 0f);
            }
            else
            {
                Debug.LogWarning($"SoundMusic file has too few lines: {lines.Length}. Using default value 0.");
                initialSoundValue = 0f;
                initialMusicValue = 0f;
            }
        }
        else
        {
            Debug.LogWarning($"SoundMusic file not found at: {filePathSoundMusic}. Using default value 0.");
            initialSoundValue = 0f;
            initialMusicValue = 0f;
        }
    }

    private void LoadKeyBindings()
    {
        if (File.Exists(filePathP1) && File.ReadAllLines(filePathP1).Length >= 4)
            initialP1Keys = File.ReadAllLines(filePathP1);
        if (File.Exists(filePathP2) && File.ReadAllLines(filePathP2).Length >= 4)
            initialP2Keys = File.ReadAllLines(filePathP2);
    }

    private void InitializeUI()
    {
        if (soundSlider != null) InitializeSlider(soundSlider, initialSoundValue);
        if (musicSlider != null) InitializeSlider(musicSlider, initialMusicValue);
        InitializeInputFields(p1Inputs, initialP1Keys);
        InitializeInputFields(p2Inputs, initialP2Keys);
    }

    private void InitializeSlider(Slider slider, float initialValue)
    {
        slider.value = initialValue >= 0 ? initialValue : 0f;
        slider.onValueChanged.Invoke(slider.value);
        LayoutRebuilder.ForceRebuildLayoutImmediate(slider.GetComponent<RectTransform>());
        Debug.Log($"Slider {slider.name} set to: {slider.value} (Loaded: {initialValue})");
    }

    private void InitializeInputFields(TMP_InputField[] inputs, string[] keys)
    {
        for (int i = 0; i < inputs.Length && i < keys.Length; i++)
        {
            if (inputs[i] != null)
            {
                string defaultText = keys[i] ?? (inputs[i].placeholder != null ? inputs[i].placeholder.GetComponent<TMP_Text>().text : "");
                if (string.IsNullOrEmpty(inputs[i].text))
                    inputs[i].text = defaultText;
                keys[i] = inputs[i].text;
            }
        }
    }

    private void SetupListeners()
    {
        if (saveButton != null) saveButton.onClick.AddListener(SaveSettings);
    }

    private float ParseFloat(string value, float defaultValue)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result)
            ? Mathf.Clamp(result, 0f, 1f)
            : defaultValue;
    }

    private void SaveSettings()
    {
        SaveKeyBindings(p1Inputs, filePathP1, initialP1Keys);
        SaveKeyBindings(p2Inputs, filePathP2, initialP2Keys);
        SaveSliderValues();
        Debug.Log($"Saved settings to {filePathSoundMusic}: Sound={soundSlider?.value ?? 0f}, Music={musicSlider?.value ?? 0f}");
    }

    private void SaveKeyBindings(TMP_InputField[] inputs, string path, string[] keys)
    {
        string[] newKeys = new string[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
            newKeys[i] = inputs[i]?.text ?? keys[i] ?? "";
        File.WriteAllLines(path, newKeys);
    }

    private void SaveSliderValues()
    {
        string[] soundMusic = {
            soundSlider != null ? soundSlider.value.ToString(CultureInfo.InvariantCulture) : "0.0",
            musicSlider != null ? musicSlider.value.ToString(CultureInfo.InvariantCulture) : "0.0"
        };
        File.WriteAllLines(filePathSoundMusic, soundMusic);
    }
}