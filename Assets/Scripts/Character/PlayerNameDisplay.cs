using UnityEngine;
using TMPro;

public class PlayerNameDisplay : MonoBehaviour
{
    [Header("UI Settings")]
    public Vector3 nameOffset = new Vector3(0, 1.5f, 0);
    public Color player1Color = Color.white;
    public Color player2Color = Color.white;
    public float fontSize = 4f;
    
    [Header("Score Display")]
    public bool showScore = true;
    public Color scoreColor = Color.yellow;
    
    private TextMeshPro nameText;
    private PlayerMovement playerMovement;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        CreateNameDisplay();
    }
    
    void CreateNameDisplay()
    {
        GameObject nameObject = new GameObject("PlayerNameDisplay");
        nameObject.transform.SetParent(transform);
        nameObject.transform.localPosition = nameOffset;
        
        nameText = nameObject.AddComponent<TextMeshPro>();
        
        nameText.text = GetPlayerDisplayText();
        nameText.fontSize = fontSize;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.sortingOrder = 32767; 
        MeshRenderer textRenderer = nameText.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingOrder = 32767;
        }
        nameText.fontStyle = FontStyles.Bold;
        if (playerMovement != null)
        {
            nameText.color = playerMovement.playerID == 1 ? player1Color : player2Color;
        }
        else
        {
            nameText.color = Color.white; 
        }
    }
    
    string GetPlayerDisplayText()
    {
        if (playerMovement == null) return "Player";
        
        string objectName = gameObject.name;
        
        if (objectName.Contains("_"))
        {
            string[] parts = objectName.Split('_');
            if (parts.Length >= 2)
            {
                string playerNum = parts[0].Replace("Player", "P");
                string characterName = parts[1];
                return $"{playerNum}\n{characterName}";
            }
        }
        
        return $"P{playerMovement.playerID}";
    }
    
    void Update()
    {
        if (nameText != null && Camera.main != null)
        {
            nameText.transform.LookAt(Camera.main.transform);
            nameText.transform.Rotate(0, 180, 0);
        }
    }
    
    public void UpdateNameDisplay()
    {
        if (nameText != null)
        {
            nameText.text = GetPlayerDisplayText();
            nameText.fontStyle = FontStyles.Bold;
            
            if (playerMovement != null)
            {
                nameText.color = playerMovement.playerID == 1 ? player1Color : player2Color;
            }
            else
            {
                nameText.color = Color.white;
            }
        }
    }
    
    public void SetNameVisible(bool visible)
    {
        if (nameText != null)
        {
            nameText.gameObject.SetActive(visible);
        }
    }
}
