using UnityEngine;

public class CharacterSpriteMaintainer : MonoBehaviour
{
    private Sprite targetSprite;
    private SpriteRenderer spriteRenderer;
    private bool isInitialized = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void SetTargetSprite(Sprite sprite)
    {
        targetSprite = sprite;
        isInitialized = true;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }
    
    void LateUpdate()
    {
        // Continuously ensure the sprite is correct (in case animations override it)
        if (isInitialized && spriteRenderer != null && targetSprite != null)
        {
            if (spriteRenderer.sprite != targetSprite)
            {
                spriteRenderer.sprite = targetSprite;
                Debug.Log($"Sprite was overridden, restoring to: {targetSprite.name}");
            }
        }
    }
}
