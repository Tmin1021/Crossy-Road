using UnityEngine;

public class MultiplayerLaneManagerSetup : MonoBehaviour
{
    void Start()
    {
        // Find LaneManager and set up player reference
        LaneManager laneManager = FindObjectOfType<LaneManager>();
        if (laneManager != null)
        {
            // Find any player (both players spawn at same time, so either works for lane spawning)
            GameObject player1 = GameObject.Find("Player1");
            GameObject player2 = GameObject.Find("Player2");
            
            if (player1 != null)
            {
                laneManager.player = player1.transform;
                Debug.Log("Assigned Player1 to LaneManager");
            }
            else if (player2 != null)
            {
                laneManager.player = player2.transform;
                Debug.Log("Assigned Player2 to LaneManager");
            }
            else
            {
                Debug.LogWarning("No players found to assign to LaneManager!");
            }
        }
        else
        {
            Debug.LogWarning("LaneManager not found in scene!");
        }
    }
}
