using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterCollection charCollection;
    public SpriteRenderer spriteRendered;
    
    [Header("Responsive Positioning")]
    public Vector2 screenPosition = new Vector2(0.5f, 0.5f); // Center of screen (0.5, 0.5)
    public float zPosition = 0f;
    
    private int selectedCharIdx = 0;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        SetupResponsivePosition();
        renderCharacterByIdx(selectedCharIdx);
    }
    
    void SetupResponsivePosition()
    {
        if (mainCamera == null) return;
        
        // Convert screen position to world position
        Vector3 screenPos = new Vector3(
            screenPosition.x * Screen.width,
            screenPosition.y * Screen.height,
            mainCamera.nearClipPlane + 1f
        );
        
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = zPosition;
        
        transform.position = worldPos;
    }
    
    void Update()
    {
        // Update position when screen size changes
        SetupResponsivePosition();
    }

    private void renderCharacterByIdx(int selectedCharIdx)
    {
        Character character = charCollection.GetCharacter(selectedCharIdx);
        spriteRendered.sprite = character.characterSprite;
    }

    public void nextChar()
    {
        selectedCharIdx++;
        if (selectedCharIdx >= charCollection.countCharacter)
            selectedCharIdx = 0;

        renderCharacterByIdx(selectedCharIdx);
    }
    
    public void previousChar()
    {
        selectedCharIdx--;
        if (selectedCharIdx < 0)
            selectedCharIdx = charCollection.countCharacter - 1;

        renderCharacterByIdx(selectedCharIdx);
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharIdx;
    }
}
