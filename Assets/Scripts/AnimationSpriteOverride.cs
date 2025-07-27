// using UnityEngine;

// public class AnimationSpriteOverride : MonoBehaviour
// {
//     private SpriteRenderer spriteRenderer;
//     private Sprite targetSprite;
    
//     void Start()
//     {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//     }
    
//     void LateUpdate()
//     {
//         // Ensure the sprite stays as intended, even if animations try to change it
//         if (spriteRenderer != null && targetSprite != null)
//         {
//             if (spriteRenderer.sprite != targetSprite)
//             {
//                 spriteRenderer.sprite = targetSprite;
//             }
//         }
//     }
    
//     public void SetTargetSprite(Sprite sprite)
//     {
//         targetSprite = sprite;
//         if (spriteRenderer != null)
//         {
//             spriteRenderer.sprite = sprite;
//         }
//     }
// }