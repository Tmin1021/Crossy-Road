using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveDistance = 1f;
    private bool _isMoving = false;
    public bool isMoving { get { return _isMoving; } }
    private Animator animator;
    private CharacterAnimationController characterAnimController;
    public LayerMask obstacleLayer;
    private Vector3 logVelocity = Vector3.zero;
    private bool onLog = false;
    [Header("Player Controls")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    [Header("Player ID")]
    public int playerID = 1;

    public AudioSource audioSource;
    public AudioClip dieSound;    // dead_chicken sound
    public AudioClip jumpSound;
    public ScoreCoinManager scoreManager;
    public LaneManager laneManager;
    public CameraAutoScroll cameraAutoScroll;
    private float lastLaneY;
    private bool isInvincible = false;  // Track invincibility status
    private float invincibilityEndTime = 10f;

    void Start()
    {
        animator = GetComponent<Animator>();
        scoreManager = FindObjectOfType<ScoreCoinManager>();
        cameraAutoScroll = FindObjectOfType<CameraAutoScroll>();
        laneManager = FindObjectOfType<LaneManager>();
        lastLaneY = transform.position.y;
        characterAnimController = GetComponent<CharacterAnimationController>();
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        LoadKeyBindings();

        // Debug obstacle layer settings
        // Debug.Log($"Player {playerID} Obstacle Detection Setup:");
        // Debug.Log($"  ObstacleLayer mask value: {obstacleLayer.value}");
        // Debug.Log($"  ObstacleLayer includes layers: {GetLayerNames(obstacleLayer)}");

        // Debug.Log($"Player {playerID} Audio Setup:");
        // Debug.Log($"  AudioSource: {(audioSource != null ? "EXISTS" : "NULL")}");
        // Debug.Log($"  JumpSound: {(jumpSound != null ? jumpSound.name : "NULL")}");
        // Debug.Log($"  DieSound: {(dieSound != null ? dieSound.name : "NULL")}");
    }

    string GetLayerNames(LayerMask layerMask)
    {
        string layerNames = "";
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) != 0)
            {
                if (layerNames.Length > 0) layerNames += ", ";
                layerNames += LayerMask.LayerToName(i);
            }
        }
        return string.IsNullOrEmpty(layerNames) ? "None" : layerNames;
    }

    void LoadKeyBindings()
    {
        Debug.Log($"LoadKeyBindings called for Player {playerID}");

        if (playerID == 1)
        {
            Debug.Log($"Player1Left key exists: {PlayerPrefs.HasKey("Player1Left")}, Value: {PlayerPrefs.GetString("Player1Left", "NOT_SET")}");
            Debug.Log($"Player1Right key exists: {PlayerPrefs.HasKey("Player1Right")}, Value: {PlayerPrefs.GetString("Player1Right", "NOT_SET")}");
            Debug.Log($"Player1Up key exists: {PlayerPrefs.HasKey("Player1Up")}, Value: {PlayerPrefs.GetString("Player1Up", "NOT_SET")}");
            Debug.Log($"Player1Down key exists: {PlayerPrefs.HasKey("Player1Down")}, Value: {PlayerPrefs.GetString("Player1Down", "NOT_SET")}");

            if (PlayerPrefs.HasKey("Player1Left"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player1Left"), out leftKey);
            if (PlayerPrefs.HasKey("Player1Right"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player1Right"), out rightKey);
            if (PlayerPrefs.HasKey("Player1Up"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player1Up"), out upKey);
            if (PlayerPrefs.HasKey("Player1Down"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player1Down"), out downKey);
        }
        else if (playerID == 2)
        {
            Debug.Log($"Player2Left key exists: {PlayerPrefs.HasKey("Player2Left")}, Value: {PlayerPrefs.GetString("Player2Left", "NOT_SET")}");
            Debug.Log($"Player2Right key exists: {PlayerPrefs.HasKey("Player2Right")}, Value: {PlayerPrefs.GetString("Player2Right", "NOT_SET")}");
            Debug.Log($"Player2Up key exists: {PlayerPrefs.HasKey("Player2Up")}, Value: {PlayerPrefs.GetString("Player2Up", "NOT_SET")}");
            Debug.Log($"Player2Down key exists: {PlayerPrefs.HasKey("Player2Down")}, Value: {PlayerPrefs.GetString("Player2Down", "NOT_SET")}");

            if (PlayerPrefs.HasKey("Player2Left"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Left"), out leftKey);
            if (PlayerPrefs.HasKey("Player2Right"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Right"), out rightKey);
            if (PlayerPrefs.HasKey("Player2Up"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Up"), out upKey);
            if (PlayerPrefs.HasKey("Player2Down"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Down"), out downKey);
        }

        Debug.Log($"Player {playerID} final keys: Up={upKey}, Down={downKey}, Left={leftKey}, Right={rightKey}");
    }

    public string GetPlayerDisplayName()
    {
        return gameObject.name;
    }

    public int GetPlayerNumber()
    {
        return playerID;
    }

    void Update()
    {
        // Don't process input if game is paused or over
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused())
        {
            return;
        }

        if (characterAnimController == null)
        {
            characterAnimController = GetComponent<CharacterAnimationController>();
        }

        if (!_isMoving)
        {
            if (Input.GetKeyDown(upKey))
            {
                // Debug.Log($"Player {playerID}: UP key pressed ({upKey})");
                StartCoroutine(Move(Vector3.up, "Up"));
            }
            else if (Input.GetKeyDown(downKey))
            {
                // Debug.Log($"Player {playerID}: DOWN key pressed ({downKey})");
                StartCoroutine(Move(Vector3.down, "Down"));
            }
            else if (Input.GetKeyDown(leftKey))
            {
                // Debug.Log($"Player {playerID}: LEFT key pressed ({leftKey})");
                StartCoroutine(Move(Vector3.left, "Left"));
            }
            else if (Input.GetKeyDown(rightKey))
            {
                // Debug.Log($"Player {playerID}: RIGHT key pressed ({rightKey})");
                StartCoroutine(Move(Vector3.right, "Right"));
            }
        }

        // if (transform.position.y >= 7 && currentLane < 7)
        // {
        //     currentLane = 7;
        //     cameraAutoScroll.StartCameraScroll(); 
        // }

        if (onLog)
        {
            transform.position += logVelocity * Time.deltaTime;
        }

        float cameraBottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        if (transform.position.y < cameraBottomY)
        {
            // resetTriggers();
            // animator.SetTrigger("Die");
            // gameObject.GetComponent<PlayerMovement>().enabled = false;
            // Debug.Log("Player has fallen off the screen!");
        }

        if (isInvincible)
        {
            // If invincibility duration has passed, disable invincibility
            if (Time.time > invincibilityEndTime)
            {
                DeactivateInvincibility();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            PlayDeathSound();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            if (characterAnimController != null)
            {
                characterAnimController.OnPlayerDie();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            // Handle coin collection logic here
            scoreManager.IncreaseCoin(1);

            Destroy(collision.gameObject);
        }
        if (isInvincible)
        {
            return;
        }
        if (collision.CompareTag("SupportItem")) 
        {
            ActivateInvincibility(10f);

            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("River"))
        {
            CheckWaterSafety();
        }
        if (collision.CompareTag("Vehicle"))
        {
            PlayDeathSound();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            if (characterAnimController != null)
            {
                characterAnimController.OnPlayerDie();
            }
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                resetTriggers();
                animator.SetTrigger("Die");
            }
            gameObject.GetComponent<PlayerMovement>().enabled = false;
        }
        if (collision.CompareTag("Log"))
        {
            LogMover logMover = collision.GetComponent<LogMover>();
            if (logMover != null)
            {
                logVelocity = logMover.direction * logMover.speed;
                onLog = true;
                Debug.Log("Player is on log. Log Speed: " + logVelocity);

            }
            CheckWaterSafety();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isInvincible)
        {
            return;
        }
        if (collision.CompareTag("River"))
        {
            if (Time.frameCount % 10 == 0)
            {
                CheckWaterSafety();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Log"))
        {
            // Only reset if not immediately entering another log
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.05f);
            bool stillOnLog = false;
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Log") && col != collision)
                {
                    stillOnLog = true;
                    LogMover logMover = col.GetComponent<LogMover>();
                    if (logMover != null)
                    {
                        logVelocity = logMover.direction * logMover.speed;
                        Debug.Log($"Player {playerID} transitioned to another log. New Log Speed: {logVelocity}");
                    }
                    break;
                }
            }

            if (!stillOnLog)
            {
                onLog = false;
                logVelocity = Vector3.zero;
                Debug.Log($"Player {playerID} exited log. Log Speed reset to: {logVelocity}");
            }
        }
    }

    IEnumerator Move(Vector3 direction, string trigger)
    {
        _isMoving = true;

        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused())
        {
            _isMoving = false;
            yield break;
        }

        PlayJumpSound();

        if (characterAnimController != null)
        {
            characterAnimController.OnPlayerMove(trigger);
        }
        else if (animator != null && animator.runtimeAnimatorController != null)
        {
            resetTriggers();
            animator.SetTrigger(trigger);
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * moveDistance;
        float elapsed = 0f;
        float duration = 0.1f;

        // Debug.Log($"Player {playerID}: Checking obstacle at position {endPos}");
        // Debug.Log($"Player {playerID}: ObstacleLayer mask = {obstacleLayer.value}");

        Collider2D hit = Physics2D.OverlapCircle(endPos, 0.05f, obstacleLayer);
        if (hit != null)
        {
            // Debug.Log($"Player {playerID}: OBSTACLE DETECTED! Hit: {hit.name} (Tag: {hit.tag}, Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
            _isMoving = false;
            yield break;
        }
        // else
        // {
        //     Debug.Log($"Player {playerID}: No obstacle detected, moving to {endPos}");
        // }

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            if (onLog)
            {
                transform.position += logVelocity * Time.deltaTime;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            transform.position.z
        );

        if (direction == Vector3.up && transform.position.y > lastLaneY)
        {
            scoreManager.IncreaseScore(1);
            lastLaneY = transform.position.y;
        }

        if (direction == Vector3.up && transform.position.y >= laneManager.lastSpawnY - 3)
        {
            laneManager.SpawnLane();
            cameraAutoScroll.MoveUpOneLane();
            laneManager.DestroyOldestLane();
        }

        _isMoving = false;
    }

    void resetTriggers()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.ResetTrigger("Up");
            animator.ResetTrigger("Down");
            animator.ResetTrigger("Left");
            animator.ResetTrigger("Right");
        }
    }

    void CheckWaterSafety()
    {
        float detectionRadius = 0.5f;
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        bool isOnSafePlatform = false;

        foreach (Collider2D col in allColliders)
        {
            int layerNumber = col.gameObject.layer;
            string layerName = LayerMask.LayerToName(layerNumber);

            if (col.CompareTag("LilyPad") || col.CompareTag("Log"))
            {
                isOnSafePlatform = true;
                break;
            }
            else if (layerName == "Lily" || layerName == "SmallLog")
            {
                isOnSafePlatform = true;
                break;
            }
        }

        if (!isOnSafePlatform)
        {
            PlayDeathSound();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            if (characterAnimController != null)
            {
                characterAnimController.OnPlayerDie();
            }
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                resetTriggers();
                animator.SetTrigger("Die");
            }
            gameObject.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    void PlayJumpSound()
    {
        // Debug.Log($"PlayJumpSound called for Player {playerID}");
        // Debug.Log($"AudioSource: {(audioSource != null ? "EXISTS" : "NULL")}");
        // Debug.Log($"JumpSound: {(jumpSound != null ? "EXISTS" : "NULL")}");

        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
            // Debug.Log($"Playing jump sound for Player {playerID}");
        }
        else
        {
            // Debug.LogWarning($"Cannot play jump sound - AudioSource: {audioSource}, JumpSound: {jumpSound}");
        }
    }

    void PlayDeathSound()
    {
        // Debug.Log($"PlayDeathSound called for Player {playerID}");
        // Debug.Log($"AudioSource: {(audioSource != null ? "EXISTS" : "NULL")}");
        // Debug.Log($"DieSound: {(dieSound != null ? "EXISTS" : "NULL")}");

        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
            // Debug.Log($"Playing death sound for Player {playerID}");
        }
        else
        {
            // Debug.LogWarning($"Cannot play death sound - AudioSource: {audioSource}, DieSound: {dieSound}");
        }
    }
    
    public void ActivateInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityEndTime = Time.time + duration;
        // Optionally, play an animation or visual effect here to indicate invincibility (e.g., a glowing effect)
        Debug.Log("Invincibility Activated!");
    }

    private void DeactivateInvincibility()
    {
        isInvincible = false;
        // Reset any visual effects for invincibility
        Debug.Log("Invincibility Deactivated!");
    }
}
