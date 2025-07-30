using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    [Header("Player Setup")]
    public GameObject playerPrefab;
    public Vector3 player1Start = new Vector3(-2, -4, 0);
    public Vector3 player2Start = new Vector3(2, -4, 0);
    
    [Header("Character Data")]
    public CharacterCollection characterCollection;
    public CharacterAnimationCollection characterAnimationCollection;

    void Start()
    {        
        int gameMode = PlayerPrefs.GetInt("SelectedGameMode", 1);
        bool isTwoPlayer = PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1;
        
        Debug.Log($"Game Mode: {gameMode}P, Two Player: {isTwoPlayer}");
        
        if (GameModeManager.Instance == null)
        {
            GameObject gameModeManagerObj = new GameObject("GameModeManager");
            gameModeManagerObj.AddComponent<GameModeManager>();
        }
        
        if (ScoreManager.Instance == null)
        {
            GameObject scoreManagerObj = new GameObject("ScoreManager");
            scoreManagerObj.AddComponent<ScoreManager>();
        }
        
        if (isTwoPlayer || gameMode == 2)
        {
            SpawnTwoPlayers();
        }
        else
        {
            SpawnOnePlayer();
        }
    }

    public void SpawnOnePlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not assigned!");
            return;
        }
        
        int characterIndex = PlayerPrefs.GetInt("Player1Character", 0);
        
        GameObject player1 = Instantiate(playerPrefab, player1Start, Quaternion.identity);
        player1.name = "Player1";
        
        PlayerMovement pm1 = player1.GetComponent<PlayerMovement>();
        pm1.upKey = KeyCode.W;
        pm1.downKey = KeyCode.S;
        pm1.leftKey = KeyCode.A;
        pm1.rightKey = KeyCode.D;
        pm1.playerID = 1;
        
        player1.AddComponent<PlayerNameDisplay>();
        StartCoroutine(ApplyCharacterDelayed(player1, characterIndex));
        
        Debug.Log($"Spawned Player 1 with character index: {characterIndex}");
    }

    public void SpawnTwoPlayers()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not assigned!");
            return;
        }
        
        int player1CharIndex = PlayerPrefs.GetInt("Player1Character", 0);
        int player2CharIndex = PlayerPrefs.GetInt("Player2Character", 0);
        
        GameObject player1 = Instantiate(playerPrefab, player1Start, Quaternion.identity);
        player1.name = "Player1";
        PlayerMovement pm1 = player1.GetComponent<PlayerMovement>();
        pm1.upKey = KeyCode.W;
        pm1.downKey = KeyCode.S;
        pm1.leftKey = KeyCode.A;
        pm1.rightKey = KeyCode.D;
        pm1.playerID = 1;
        player1.AddComponent<PlayerNameDisplay>();
        ApplyCharacterToPlayer(player1, player1CharIndex);

        GameObject player2 = Instantiate(playerPrefab, player2Start, Quaternion.identity);
        player2.name = "Player2";
        PlayerMovement pm2 = player2.GetComponent<PlayerMovement>();
        pm2.upKey = KeyCode.UpArrow;
        pm2.downKey = KeyCode.DownArrow;
        pm2.leftKey = KeyCode.LeftArrow;
        pm2.rightKey = KeyCode.RightArrow;
        pm2.playerID = 2;
        player2.AddComponent<PlayerNameDisplay>();
        StartCoroutine(ApplyCharacterDelayed(player2, player2CharIndex));
        
        Debug.Log($"Spawned 2 players with character indices: P1={player1CharIndex}, P2={player2CharIndex}");
    }
    
    void ApplyCharacterToPlayer(GameObject player, int characterIndex)
    {
        Debug.Log($"Applying character index {characterIndex} to {player.name}");
        
        if (characterCollection == null)
        {
            Debug.LogWarning("CharacterCollection not assigned to MultiplayerManager!");
            return;
        }
        
        Debug.Log($"CharacterCollection has {characterCollection.countCharacter} characters");
        
        if (characterIndex >= 0 && characterIndex < characterCollection.countCharacter)
        {
            Character character = characterCollection.GetCharacter(characterIndex);
            Debug.Log($"Got character: {character.characterName} with sprite: {character.characterSprite?.name}");
            
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && character.characterSprite != null)
            {
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
