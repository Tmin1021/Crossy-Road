// using UnityEngine;

// public class SupportItem : MonoBehaviour
// {
//     public float invincibilityDuration = 10f;  // Duration of invincibility in seconds
    
//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             PlayerMovement player = collision.GetComponent<PlayerMovement>();
//             Debug.Log("Support item collected!");
//             if (player != null)
//             {
//                 player.ActivateInvincibility(invincibilityDuration);
//             }

//             // Destroy the support item after it's collected
//             Destroy(gameObject);
//         }
//     }
// }
