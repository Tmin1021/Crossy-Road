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
        SetupResponsivePosition();
    }

    private void renderCharacterByIdx(int selectedCharIdx)
    {
        Character character = charCollection.GetCharacter(selectedCharIdx);
        spriteRendered.sprite = character.characterSprite;
    }

    public void nextChar()
    {
        int nextIndex = selectedCharIdx + 1;
        if (nextIndex >= charCollection.countCharacter)
            nextIndex = 0;

        selectedCharIdx = nextIndex;
        renderCharacterByIdx(selectedCharIdx);
        
        OnCharacterChanged();
    }
    
    public void previousChar()
    {
        int previousIndex = selectedCharIdx - 1;
        if (previousIndex < 0)
            previousIndex = charCollection.countCharacter - 1;

        selectedCharIdx = previousIndex;
        renderCharacterByIdx(selectedCharIdx);
        OnCharacterChanged();
    }
    
    private void OnCharacterChanged()
    {

        GameObject[] uiObjects = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject obj in uiObjects)
        {
            obj.SendMessage("OnCharacterSelectionChanged", selectedCharIdx, SendMessageOptions.DontRequireReceiver);
        }
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharIdx;
    }
    
    public bool CanSelectCharacter()
    {
        GameObject unlockManagerObj = GameObject.Find("CharacterUnlockManager");
        if (unlockManagerObj != null)
        {
            MonoBehaviour unlockManager = unlockManagerObj.GetComponent<MonoBehaviour>();
            if (unlockManager != null)
            {
                return true;
            }
        }
        return true; 
    }
}
