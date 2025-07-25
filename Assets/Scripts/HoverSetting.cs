using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Globalization;

public class HoverSetting : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image soundImage;      // Reference to the Image component for sound icon
    public Sprite soundNormal;    // Normal sound sprite
    public Sprite soundHover;     // Hover sound sprite
    public Sprite soundMuted;     // Muted sound sprite
    public Slider soundSlider;    // Reference to the Slider component
    public string sliderType;     // Inspector field to select 'sound' or 'music' (line 0 or 1)
    private string filePath = Path.Combine(Application.streamingAssetsPath, "sound_music.txt");
    private bool isDragging = false; // To check if the slider is being dragged

    void Start()
    {
        // Ensure the image starts with the normal sprite
        soundImage.sprite = soundNormal;

        // Load the initial slider value from the .txt file
        LoadSliderValue();

        // Add listener to the slider to detect value changes
        soundSlider.onValueChanged.AddListener(OnSliderValueChanged);

        // Call the method once at start to ensure the image is correct
        UpdateImageBasedOnSlider();
    }

    private void LoadSliderValue()
    {
        if (string.IsNullOrEmpty(sliderType) || !File.Exists(filePath))
        {
            Debug.LogWarning($"Slider type not set or file not found at: {filePath}. Using default value 1.");
            soundSlider.value = 1f; // Default to max if file or type is invalid
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        int index = sliderType.ToLower() == "sound" ? 0 : (sliderType.ToLower() == "music" ? 1 : -1);

        if (index >= 0 && index < lines.Length)
        {
            if (float.TryParse(lines[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float loadedValue))
            {
                soundSlider.value = Mathf.Clamp(loadedValue, 0f, 1f);
                Debug.Log($"Slider {sliderType} set to: {soundSlider.value} (Loaded: {loadedValue})");
            }
            else
            {
                Debug.LogError($"Failed to parse {sliderType} value from: '{lines[index]}'");
                soundSlider.value = 1f; // Default to max if parsing fails
            }
        }
        else
        {
            Debug.LogError($"Invalid slider type '{sliderType}' or insufficient lines in {filePath}. Using default value 1.");
            soundSlider.value = 1f;
        }
    }

    // This function is called whenever the slider value changes
    public void OnSliderValueChanged(float value)
    {
        if (isDragging) // Keep the hover effect while dragging
        {
            soundImage.sprite = soundHover; // Show hover effect while dragging
        }
        else // When dragging stops, revert to normal or muted
        {
            UpdateImageBasedOnSlider();
        }
    }

    // This function updates the image based on the slider value
    private void UpdateImageBasedOnSlider()
    {
        float value = soundSlider.value;

        if (value == 0)
        {
            soundImage.sprite = soundMuted;
        }
        else
        {
            soundImage.sprite = soundNormal;
        }
    }

    // This method is called when the user starts dragging (pointer down)
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        soundImage.sprite = soundHover;
    }

    // This method is called when the user releases the slider (pointer up)
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        UpdateImageBasedOnSlider();
    }
}