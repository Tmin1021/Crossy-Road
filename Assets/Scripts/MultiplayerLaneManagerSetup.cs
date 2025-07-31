using UnityEngine;
using System.Collections;

public class MultiplayerLaneManagerSetup : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SetupLaneManagerDelayed());
    }
    
    System.Collections.IEnumerator SetupLaneManagerDelayed()
    {
        Debug.Log("MultiplayerLaneManagerSetup: Starting delayed setup...");
        
        yield return new WaitForSeconds(1f);
        
        LaneManager laneManager = FindObjectOfType<LaneManager>();
        Debug.Log($"LaneManager found: {laneManager != null}");
        
        if (laneManager != null)
        {
            GameObject player1 = GameObject.Find("Player1");
            GameObject player2 = GameObject.Find("Player2");
            
            Debug.Log($"Player1 found: {player1 != null}");
            Debug.Log($"Player2 found: {player2 != null}");
            
            if (player1 != null)
            {
                laneManager.player = player1.transform;
                Debug.Log($"SUCCESS: Assigned Player1 to LaneManager. Player position: {player1.transform.position}");
            }
            else if (player2 != null)
            {
                laneManager.player = player2.transform;
                Debug.Log($"SUCCESS: Assigned Player2 to LaneManager. Player position: {player2.transform.position}");
            }
            else
            {
                Debug.LogError("ERROR: No players found to assign to LaneManager!");
                
                PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
                Debug.Log($"Found {allPlayers.Length} PlayerMovement components in scene");
                
                if (allPlayers.Length > 0)
                {
                    laneManager.player = allPlayers[0].transform;
                    Debug.Log($"FALLBACK: Assigned {allPlayers[0].name} to LaneManager");
                }
            }
        }
        else
        {
            Debug.LogError("ERROR: LaneManager not found in scene!");
        }
    }
}
