using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private CharacterAnimationSet currentAnimationSet;
    
    [Header("Animation Settings")]
    public CharacterAnimationCollection animationCollection;
    
    private string lastDirection = "";
    private bool wasMoving = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        
        Debug.Log($"CharacterAnimationController Start - SpriteRenderer: {spriteRenderer != null}, PlayerMovement: {playerMovement != null}");
        
        if (animationCollection == null)
        {
            Debug.LogWarning("CharacterAnimationCollection not assigned to CharacterAnimationController!");
        }
        else
        {
            Debug.Log("CharacterAnimationCollection is assigned!");
        }
    }
    
    void Update()
    {
        if (currentAnimationSet == null || playerMovement == null) return;
        
        bool isMoving = playerMovement.isMoving;
        // if (isMoving != wasMoving)
        // {
        //     Debug.Log($"Movement state changed: {wasMoving} -> {isMoving}");
        // } 
        wasMoving = isMoving;
    }
    
    public void SetCharacterAnimationSet(int characterIndex)
    {
        if (animationCollection != null)
        {
            currentAnimationSet = animationCollection.GetCharacterAnimationSet(characterIndex);
            if (currentAnimationSet != null)
            {
                // Set initial idle sprite
                SetSprite(currentAnimationSet.idleSprite);
                // Debug.Log($"Set animation set for character: {currentAnimationSet.characterName}");
                // Debug.Log($"Available sprites - Idle: {currentAnimationSet.idleSprite?.name}, Back: {currentAnimationSet.backSprite?.name}, Front: {currentAnimationSet.frontSprite?.name}, Left: {currentAnimationSet.leftSprite?.name}, Right: {currentAnimationSet.rightSprite?.name}, Die: {currentAnimationSet.dieSprite?.name}");
            }
            // else
            // {
            //     Debug.LogWarning($"No animation set found for character index: {characterIndex}");
            // }
        }
        // else
        // {
        //     Debug.LogWarning("CharacterAnimationCollection is null - cannot set character animation set");
        // }
    }
    
    public void SetCharacterAnimationSet(string characterName)
    {
        if (animationCollection != null)
        {
            currentAnimationSet = animationCollection.GetCharacterAnimationSet(characterName);
            if (currentAnimationSet != null)
            {
                // Set initial idle sprite
                SetSprite(currentAnimationSet.idleSprite);
                // Debug.Log($"Set animation set for character: {currentAnimationSet.characterName}");
            }
            else
            {
                // Debug.LogWarning($"No animation set found for character: {characterName}");
            }
        }
    }
    
    // Call this method when player moves in a direction
    public void OnPlayerMove(string direction)
    {
        if (currentAnimationSet == null) 
        {
            // Debug.LogWarning("OnPlayerMove called but currentAnimationSet is null!");
            return;
        }
        
        lastDirection = direction;
        // Debug.Log($"OnPlayerMove called with direction: {direction}");
        
        switch (direction.ToLower())
        {
            case "up":
                SetSprite(currentAnimationSet.backSprite);
                break;
            case "down":
                SetSprite(currentAnimationSet.frontSprite);
                break;
            case "left":
                SetSprite(currentAnimationSet.leftSprite);
                break;
            case "right":
                SetSprite(currentAnimationSet.rightSprite);
                break;
            default:
                SetSprite(currentAnimationSet.idleSprite);
                break;
        }
    }
    
    private void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"SetSprite failed - SpriteRenderer: {spriteRenderer != null}, Sprite: {sprite != null}");
        }
    }
    
    // Get the current character animation set (for external use)
    public CharacterAnimationSet GetCurrentAnimationSet()
    {
        return currentAnimationSet;
    }
    
    // Call this method when player dies
    public void OnPlayerDie()
    {
        if (currentAnimationSet == null) 
        {
            Debug.LogWarning("OnPlayerDie called but currentAnimationSet is null!");
            return;
        }
        
        if (currentAnimationSet.dieSprite != null)
        {
            SetSprite(currentAnimationSet.dieSprite);
        }
        else
        {
            // Fallback to idle sprite if no die sprite is assigned
            SetSprite(currentAnimationSet.idleSprite);
        }
    }
}
