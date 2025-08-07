using System.Collections;
using System.Runtime.CompilerServices;
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
    [Header("Aura")]
    public GameObject shieldAura;
    public GameObject magnetAura;


    public AudioSource audioSource;
    public AudioClip dieSound;    
    public AudioClip jumpSound;
    public ScoreCoinManager scoreManager;
    public LaneManager laneManager;
    public CameraAutoScroll cameraAutoScroll;
    private SpriteRenderer sr;
    private float lastLaneY;
    private bool isInvincible = false;
    private float invincibilityEndTime;
    private bool isMagnetEffect = false;
    private float magnetEffectEndTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        scoreManager = FindObjectOfType<ScoreCoinManager>();
        cameraAutoScroll = FindObjectOfType<CameraAutoScroll>();
        laneManager = FindObjectOfType<LaneManager>();
        lastLaneY = transform.position.y;
        characterAnimController = GetComponent<CharacterAnimationController>();
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        sr = GetComponent<SpriteRenderer>();
        LoadKeyBindings();
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
                StartCoroutine(Move(Vector3.up, "Up"));
            }
            else if (Input.GetKeyDown(downKey))
            {
                StartCoroutine(Move(Vector3.down, "Down"));
            }
            else if (Input.GetKeyDown(leftKey))
            {
                StartCoroutine(Move(Vector3.left, "Left"));
            }
            else if (Input.GetKeyDown(rightKey))
            {
                StartCoroutine(Move(Vector3.right, "Right"));
            }
        }

        if (onLog)
        {
            transform.position += logVelocity * Time.deltaTime;
        }

        float cameraBottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        if (transform.position.y < cameraBottomY
        || transform.position.x < -9f
        || transform.position.x > 9f)
        {
            PlayerDie();
        }

        if (isInvincible)
        {
            if (Time.time > invincibilityEndTime)
            {
                DeactivateInvincibility();
            }
        }

        if (isMagnetEffect)
        {
            CoinMovement();
            if (Time.time > magnetEffectEndTime)
            {
                isMagnetEffect = false;
                magnetAura.SetActive(false);
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
            scoreManager.IncreaseCoin(1);

            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Magnet"))
        {
            isMagnetEffect = true;
            magnetEffectEndTime = Time.time + 10f;

            if (magnetAura != null)
            {
                magnetAura.SetActive(true);
                Debug.Log("Magnet Aura On");
            }

            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Shield"))
        {
            ActivateInvincibility(5f);

            Destroy(collision.gameObject);
        } 

        if (collision.CompareTag("Log"))
        {
            LogMover logMover = collision.GetComponent<LogMover>();
            if (logMover != null)
            {
                logVelocity = logMover.direction * logMover.speed;
                onLog = true;
            }
            CheckWaterSafety();
        }

        if (isInvincible)
        {
            return;
        }

        if (collision.CompareTag("River"))
        {
            CheckWaterSafety();
        }
        if (collision.CompareTag("Vehicle"))
        {
            PlayerDie();
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

        Collider2D hit = Physics2D.OverlapCircle(endPos, 0.05f, obstacleLayer);
        if (hit != null)
        {
            _isMoving = false;
            yield break;
        }

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

        if (direction == Vector3.up && transform.position.y >= laneManager.getLastSpawnY() - 5)
        {
            laneManager.SpawnLane();
            cameraAutoScroll.MoveUpOneLane();
            laneManager.DestroyOldestLane();
        }

        _isMoving = false;
    }

    public string GetPlayerDisplayName()
    {
        return gameObject.name;
    }

    public int GetPlayerNumber()
    {
        return playerID;
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
    void LoadKeyBindings()
    {
        if (playerID == 1)
        {
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
            if (PlayerPrefs.HasKey("Player2Left"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Left"), out leftKey);
            if (PlayerPrefs.HasKey("Player2Right"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Right"), out rightKey);
            if (PlayerPrefs.HasKey("Player2Up"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Up"), out upKey);
            if (PlayerPrefs.HasKey("Player2Down"))
                System.Enum.TryParse(PlayerPrefs.GetString("Player2Down"), out downKey);
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
            PlayerDie();
        }
    }

    void PlayerDie()
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
        
        if (shieldAura != null)
        {
            shieldAura.SetActive(true);
        }
    }

    private void DeactivateInvincibility()
    {
        isInvincible = false;
        
        if (shieldAura != null)
        {
            shieldAura.SetActive(false);
        }
    }
    
    private void CoinMovement()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        if (coins.Length == 0)
            return;

        foreach (GameObject coin in coins)
        {
            if (coin != null)
            {
                Vector3 coinPos = coin.transform.position;
                Vector3 playerPos = transform.position;

                coin.transform.position = Vector3.MoveTowards(coinPos, playerPos, 10f * Time.deltaTime);
            }
        }
    }
}
