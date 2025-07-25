using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite defaultSprite; // Assign the white sprite in Inspector
    public Sprite highlightedSprite; // Assign the orange sprite in Inspector
    public AudioClip hoverSound; // Assign the hover sound in Inspector
    public AudioClip clickSound; // Assign the click sound in Inspector
    private Image buttonImage;
    private AudioSource audioSource;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        if (buttonImage == null)
        {
            Debug.LogError("Image component not found on the button!");
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the button!");
        }
        // Set default sprite initially
        buttonImage.sprite = defaultSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change to orange sprite and play hover sound when mouse enters
        Debug.LogError("Hover");
        buttonImage.sprite = highlightedSprite;
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Change back to white sprite when mouse exits
        buttonImage.sprite = defaultSprite;
    }

    // Handle click sound (requires Button component's OnClick event)
    public void OnButtonClick()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
            Debug.Log("Button clicked and click sound played");
        }
        else
        {
            Debug.Log("Click sound or AudioSource is null");
        }
    }
}