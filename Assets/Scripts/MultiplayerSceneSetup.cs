using UnityEngine;
using System.Collections;

public class MultiplayerSceneSetup : MonoBehaviour
{
    [Header("Required References - Assign in Inspector")]
    public GameObject playerPrefab;
    public CharacterCollection characterCollection;
    public CharacterAnimationCollection characterAnimationCollection;
    
    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip dieSound;

    [Header("Spawn Settings")]
    public Vector3 player1SpawnPos = new Vector3(-2, -4, 0);
    public Vector3 player2SpawnPos = new Vector3(2, -4, 0);

    void Start()
    {
        Debug.Log("MultiplayerSceneSetup: Initializing multiplayer scene...");
        int gameMode = PlayerPrefs.GetInt("SelectedGameMode", 2);
        bool isTwoPlayer = PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1;
        int player1CharIndex = PlayerPrefs.GetInt("Player1Character", 0);
        int player2CharIndex = PlayerPrefs.GetInt("Player2Character", 0);

        Debug.Log($"Game Mode: {gameMode}P, Two Player: {isTwoPlayer}");
        Debug.Log($"Player 1 Character: {player1CharIndex}");
        Debug.Log($"Player 2 Character: {player2CharIndex}");

        if (isTwoPlayer || gameMode == 2)
        {
            SpawnTwoPlayers();
        }
        else
        {
            SpawnOnePlayer();
        }
        
        StartCoroutine(SetupLaneManagerDelayed());
    }
    
    IEnumerator SetupLaneManagerDelayed()
    {
        // Debug.Log("MultiplayerSceneSetup: Starting LaneManager setup...");
        
        yield return new WaitForSeconds(0.5f);
        
        LaneManager laneManager = FindObjectOfType<LaneManager>();
        // Debug.Log($"LaneManager found: {laneManager != null}");
        
        if (laneManager != null)
        {
            GameObject player1 = GameObject.Find("Player1");
            GameObject player2 = GameObject.Find("Player2");
            
            // Debug.Log($"Player1 found: {player1 != null}");
            // Debug.Log($"Player2 found: {player2 != null}");
            
            if (player1 != null)
            {
                laneManager.player = player1.transform;
                // Debug.Log($"MultiplayerSceneSetup: Assigned Player1 to LaneManager at position {player1.transform.position}");
            }
            else if (player2 != null)
            {
                laneManager.player = player2.transform;
                Debug.Log($"MultiplayerSceneSetup: Assigned Player2 to LaneManager at position {player2.transform.position}");
            }
            else
            {
                Debug.LogError("MultiplayerSceneSetup: No players found to assign to LaneManager!");
            }
        }
        else
        {
            Debug.LogError("MultiplayerSceneSetup: LaneManager not found in scene!");
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
        
        PlayerMovement pm1 = player1.GetComponent<PlayerMovement>();
        if (pm1 != null)
        {
            pm1.playerID = 1;
            pm1.upKey = KeyCode.W;
            pm1.downKey = KeyCode.S;
            pm1.leftKey = KeyCode.A;
            pm1.rightKey = KeyCode.D;
            
            AudioSource audioSource = player1.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = player1.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.7f;
                Debug.Log("Added AudioSource to Player1");
            }
            pm1.audioSource = audioSource;
            
            if (jumpSound != null) pm1.jumpSound = jumpSound;
            if (dieSound != null) pm1.dieSound = dieSound;
        }
        
        if (player1.GetComponent<PlayerNameDisplay>() == null)
        {
            player1.AddComponent<PlayerNameDisplay>();
        }
        
        StartCoroutine(ApplyCharacterDelayed(player1, characterIndex));
        
        // Debug.Log($"Spawned Player 1 with character index: {characterIndex}");
    }

    void SpawnTwoPlayers()
    {
        if (playerPrefab == null)
        {
            // Debug.LogError("Player Prefab not assigned!");
            return;
        }
        
        int player1CharIndex = PlayerPrefs.GetInt("Player1Character", 0);
        int player2CharIndex = PlayerPrefs.GetInt("Player2Character", 0);
        
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
            
            AudioSource audioSource1 = player1.GetComponent<AudioSource>();
            if (audioSource1 == null)
            {
                audioSource1 = player1.AddComponent<AudioSource>();
                audioSource1.playOnAwake = false;
                audioSource1.volume = 0.7f;
                // ebug.Log("Added AudioSource to Player1");
            }
            pm1.audioSource = audioSource1;
            
            // Assign audio clips if available
            if (jumpSound != null) pm1.jumpSound = jumpSound;
            if (dieSound != null) pm1.dieSound = dieSound;
        }
        if (player1.GetComponent<PlayerNameDisplay>() == null)
        {
            player1.AddComponent<PlayerNameDisplay>();
        }
        StartCoroutine(ApplyCharacterDelayed(player1, player1CharIndex));

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
            
            AudioSource audioSource2 = player2.GetComponent<AudioSource>();
            if (audioSource2 == null)
            {
                audioSource2 = player2.AddComponent<AudioSource>();
                audioSource2.playOnAwake = false;
                audioSource2.volume = 0.7f;
            }
            pm2.audioSource = audioSource2;
            
            if (jumpSound != null) pm2.jumpSound = jumpSound;
            if (dieSound != null) pm2.dieSound = dieSound;
        }
        if (player2.GetComponent<PlayerNameDisplay>() == null)
        {
            player2.AddComponent<PlayerNameDisplay>();
        }
        StartCoroutine(ApplyCharacterDelayed(player2, player2CharIndex));
        
        // Debug.Log($"Spawned 2 players with character indices: P1={player1CharIndex}, P2={player2CharIndex}");
    }

    void ApplyCharacterToPlayer(GameObject player, int characterIndex)
    {
        if (characterCollection == null)
        {
            Debug.LogError("CharacterCollection not assigned to MultiplayerSceneSetup!");
            return;
        }
        

        if (characterIndex >= 0 && characterIndex < characterCollection.countCharacter)
        {
            Character character = characterCollection.GetCharacter(characterIndex);
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && character.characterSprite != null)
            {
                Sprite oldSprite = spriteRenderer.sprite;
                spriteRenderer.sprite = character.characterSprite;
                if (characterAnimationCollection != null)
                {
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
