using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameUI : MonoBehaviour
{
    [Header("UI Settings")]
    public Vector3 nameOffset = new Vector3(0, 50f, 0);
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;
    public float fontSize = 14f;
    
    private GameObject nameUI;
    private TextMeshProUGUI nameText;
    private PlayerMovement playerMovement;
    private Canvas worldCanvas;
    private Camera mainCamera;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        mainCamera = Camera.main;
        CreateNameUI();
    }
    
    void CreateNameUI()
    {
        GameObject canvasObject = new GameObject("PlayerNameCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.transform.localPosition = Vector3.zero;
        
        worldCanvas = canvasObject.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.worldCamera = mainCamera;
        
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObject.AddComponent<GraphicRaycaster>();
        
        nameUI = new GameObject("PlayerNameText");
        nameUI.transform.SetParent(canvasObject.transform);
        
        // Add and configure TextMeshProUGUI
        nameText = nameUI.AddComponent<TextMeshProUGUI>();
        nameText.text = GetPlayerDisplayText();
        nameText.fontSize = fontSize;
        nameText.alignment = TextAlignmentOptions.Center;
        
        // Set color based on player ID
        if (playerMovement != null)
        {
            nameText.color = playerMovement.playerID == 1 ? player1Color : player2Color;
        }
        
        // Position the text above the player
        RectTransform textRect = nameText.GetComponent<RectTransform>();
        textRect.anchoredPosition = nameOffset;
        textRect.sizeDelta = new Vector2(200, 50);
        
        // Scale the canvas to be small in world space
        canvasObject.transform.localScale = Vector3.one * 0.01f;
        
        Debug.Log($"Created UI name display for {gameObject.name}: {nameText.text}");
    }
    
    string GetPlayerDisplayText()
    {
        if (playerMovement == null) return "Player";
        
        // Extract character name from GameObject name
        string objectName = gameObject.name;
        
        if (objectName.Contains("_"))
        {
            // Split "Player1_Chicken" into parts
            string[] parts = objectName.Split('_');
            if (parts.Length >= 2)
            {
                string playerNum = parts[0].Replace("Player", "P"); // "Player1" -> "P1"
                string characterName = parts[1]; // "Chicken"
                return $"{playerNum} - {characterName}"; // One line: "P1 - Chicken"
            }
        }
        
        // Fallback
        return $"Player {playerMovement.playerID}";
    }
    
    void Update()
    {
        // Make the canvas face the camera
        if (worldCanvas != null && mainCamera != null)
        {
            worldCanvas.transform.LookAt(mainCamera.transform);
            worldCanvas.transform.Rotate(0, 180, 0); // Flip to face correctly
        }
    }
    
    // Public method to update the display if needed
    public void UpdateNameDisplay()
    {
        if (nameText != null)
        {
            nameText.text = GetPlayerDisplayText();
            if (playerMovement != null)
            {
                nameText.color = playerMovement.playerID == 1 ? player1Color : player2Color;
            }
        }
    }
    
    // Public method to show/hide the name display
    public void SetNameVisible(bool visible)
    {
        if (nameUI != null)
        {
            nameUI.SetActive(visible);
        }
    }
}
