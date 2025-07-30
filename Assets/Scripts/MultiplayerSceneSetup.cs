using UnityEngine;
using System.Collections;

public class MultiplayerSceneSetup : MonoBehaviour
{
    [Header("Required References - Assign in Inspector")]
    public GameObject playerPrefab;
    public CharacterCollection characterCollection;
    public CharacterAnimationCollection characterAnimationCollection;

    [Header("Spawn Settings")]
    public Vector3 player1SpawnPos = new Vector3(-2, -4, 0);
    public Vector3 player2SpawnPos = new Vector3(2, -4, 0);

    void Start()
    {
        Debug.Log("MultiplayerSceneSetup: Initializing multiplayer scene...");

        // Initialize managers first (comment out if not available)
        // if (GameModeManager.Instance == null)
        // {
        //     GameObject gameModeManagerObj = new GameObject("GameModeManager");
        //     gameModeManagerObj.AddComponent<GameModeManager>();
        // }
        
        // if (ScoreManager.Instance == null)
        // {
        //     GameObject scoreManagerObj = new GameObject("ScoreManager");
        //     scoreManagerObj.AddComponent<ScoreManager>();
        // }

        // Load saved settings
        int gameMode = PlayerPrefs.GetInt("SelectedGameMode", 2);
        bool isTwoPlayer = PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1;
        int player1CharIndex = PlayerPrefs.GetInt("Player1Character", 0);
        int player2CharIndex = PlayerPrefs.GetInt("Player2Character", 0);

        Debug.Log($"Game Mode: {gameMode}P, Two Player: {isTwoPlayer}");
        Debug.Log($"Player 1 Character: {player1CharIndex}");
        Debug.Log($"Player 2 Character: {player2CharIndex}");

        // Spawn players based on mode
        if (isTwoPlayer || gameMode == 2)
        {
            SpawnTwoPlayers();
        }
        else
        {
            SpawnOnePlayer();
        }
        
        // Fix LaneManager player reference after spawning
        StartCoroutine(SetupLaneManagerDelayed());
    }
    
    System.Collections.IEnumerator SetupLaneManagerDelayed()
    {
        // Wait for players to be fully spawned
        yield return new WaitForEndOfFrame();
        
        LaneManager laneManager = FindObjectOfType<LaneManager>();
        if (laneManager != null)
        {
            // Find any player for lane spawning reference
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
    }

    void SpawnOnePlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not assigned!");
            return;
        }
        
        int characterIndex = PlayerPrefs.GetInt("Player1Character", 0);
        
        GameObject player1 = Instantiate(playerPrefab, player1SpawnPos, Quaternion.identity);
        player1.name = "Player1";
        
        // Setup PlayerMovement with default keys (will be overridden by LoadKeyBindings)
        PlayerMovement pm1 = player1.GetComponent<PlayerMovement>();
        if (pm1 != null)
        {
            pm1.playerID = 1;
            pm1.upKey = KeyCode.W;
            pm1.downKey = KeyCode.S;
            pm1.leftKey = KeyCode.A;
            pm1.rightKey = KeyCode.D;
        }
        
        // Add name display
        if (player1.GetComponent<PlayerNameDisplay>() == null)
        {
            player1.AddComponent<PlayerNameDisplay>();
        }
        
        // Apply character with delay for better reliability
        StartCoroutine(ApplyCharacterDelayed(player1, characterIndex));
        
        Debug.Log($"Spawned Player 1 with character index: {characterIndex}");
    }

    void SpawnTwoPlayers()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not assigned!");
            return;
        }
        
        int player1CharIndex = PlayerPrefs.GetInt("Player1Character", 0);
        int player2CharIndex = PlayerPrefs.GetInt("Player2Character", 0);
        
        // Spawn Player 1
        GameObject player1 = Instantiate(playerPrefab, player1SpawnPos, Quaternion.identity);
        player1.name = "Player1";
        PlayerMovement pm1 = player1.GetComponent<PlayerMovement>();
        if (pm1 != null)
        {
            pm1.playerID = 1;
            pm1.upKey = KeyCode.W;
            pm1.downKey = KeyCode.S;
            pm1.leftKey = KeyCode.A;
            pm1.rightKey = KeyCode.D;
        }
        if (player1.GetComponent<PlayerNameDisplay>() == null)
        {
            player1.AddComponent<PlayerNameDisplay>();
        }
        StartCoroutine(ApplyCharacterDelayed(player1, player1CharIndex));

        // Spawn Player 2
        GameObject player2 = Instantiate(playerPrefab, player2SpawnPos, Quaternion.identity);
        player2.name = "Player2";
        PlayerMovement pm2 = player2.GetComponent<PlayerMovement>();
        if (pm2 != null)
        {
            pm2.playerID = 2;
            pm2.upKey = KeyCode.UpArrow;
            pm2.downKey = KeyCode.DownArrow;
            pm2.leftKey = KeyCode.LeftArrow;
            pm2.rightKey = KeyCode.RightArrow;
        }
        if (player2.GetComponent<PlayerNameDisplay>() == null)
        {
            player2.AddComponent<PlayerNameDisplay>();
        }
        StartCoroutine(ApplyCharacterDelayed(player2, player2CharIndex));
        
        Debug.Log($"Spawned 2 players with character indices: P1={player1CharIndex}, P2={player2CharIndex}");
    }

    void ApplyCharacterToPlayer(GameObject player, int characterIndex)
    {
        Debug.Log($"Applying character index {characterIndex} to {player.name}");
        
        if (characterCollection == null)
        {
            Debug.LogWarning("CharacterCollection not assigned to MultiplayerSceneSetup!");
            return;
        }
        
        Debug.Log($"CharacterCollection has {characterCollection.countCharacter} characters");

        if (characterIndex >= 0 && characterIndex < characterCollection.countCharacter)
        {
            Character character = characterCollection.GetCharacter(characterIndex);
            Debug.Log($"Got character: {character.characterName} with sprite: {character.characterSprite?.name}");

            // Apply sprite to player
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && character.characterSprite != null)
            {
                spriteRenderer.sprite = character.characterSprite;
                Debug.Log($"Applied character sprite '{character.characterName}' to {player.name}");
                
                // Setup character animations if available
                if (characterAnimationCollection != null)
                {
                    // Remove old animation controller if exists
                    PlayerAnimationController oldAnimController = player.GetComponent<PlayerAnimationController>();
                    if (oldAnimController != null)
                    {
                        DestroyImmediate(oldAnimController);
                    }
                    
                    CharacterAnimationController animController = player.GetComponent<CharacterAnimationController>();
                    if (animController == null)
                    {
                        animController = player.AddComponent<CharacterAnimationController>();
                    }
                    
                    animController.animationCollection = characterAnimationCollection;
                    animController.SetCharacterAnimationSet(characterIndex);
                    Debug.Log($"Setup character animations for {player.name}");
                }
                else
                {
                    Debug.LogWarning("CharacterAnimationCollection not assigned - using basic sprite only");
                }
                
                // Disable default animator if using character animations
                Animator playerAnimator = player.GetComponent<Animator>();
                if (playerAnimator != null)
                {
                    playerAnimator.runtimeAnimatorController = null;
                }
            }
            else
            {
                Debug.LogWarning($"SpriteRenderer: {spriteRenderer != null}, Character sprite: {character.characterSprite != null}");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid character index: {characterIndex}, available characters: {characterCollection.countCharacter}");
        }
    }

    System.Collections.IEnumerator ApplyCharacterDelayed(GameObject player, int characterIndex)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        ApplyCharacterToPlayer(player, characterIndex);
        
        yield return new WaitForSeconds(0.1f);
        ApplyCharacterToPlayer(player, characterIndex);
    }
}
