using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f;
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
    public ScoreManager scoreManager;
    public LaneManager laneManager;
    public CameraAutoScroll cameraAutoScroll;
    private float lastLaneY;

    void Start()
    {
        animator = GetComponent<Animator>();
        scoreManager = FindObjectOfType<ScoreManager>();
        cameraAutoScroll = FindObjectOfType<CameraAutoScroll>();
        laneManager = FindObjectOfType<LaneManager>();
        lastLaneY = transform.position.y;
        characterAnimController = GetComponent<CharacterAnimationController>();
        LoadKeyBindings();
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
        if (characterAnimController == null)
        {
            characterAnimController = GetComponent<CharacterAnimationController>();
        }

        if (!_isMoving)
        {
            if (Input.GetKeyDown(upKey))
            {
                Debug.Log($"Player {playerID} pressed UP key ({upKey})");
                StartCoroutine(Move(Vector3.up, "Up"));
            }
            else if (Input.GetKeyDown(downKey))
            {
                Debug.Log($"Player {playerID} pressed DOWN key ({downKey})");
                StartCoroutine(Move(Vector3.down, "Down"));
            }
            else if (Input.GetKeyDown(leftKey))
            {
                Debug.Log($"Player {playerID} pressed LEFT key ({leftKey})");
                StartCoroutine(Move(Vector3.left, "Left"));
            }
            else if (Input.GetKeyDown(rightKey))
            {
                Debug.Log($"Player {playerID} pressed RIGHT key ({rightKey})");
                StartCoroutine(Move(Vector3.right, "Right"));
            }
        }

        // if (transform.position.y >= 7 && currentLane < 7)
        // {
        //     currentLane = 7;
        //     cameraAutoScroll.StartCameraScroll(); 
        // }

        float cameraBottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;

        if (onLog)
        {
            transform.position += logVelocity * Time.deltaTime;
        }

        if (transform.position.y < cameraBottomY)
        {
            // resetTriggers();
            // animator.SetTrigger("Die");
            // gameObject.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Player has fallen off the screen!");
        }
    }

    IEnumerator Move(Vector3 direction, string trigger)
    {
        _isMoving = true;

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
            Debug.Log("DEATH CAUSE: DROWNING - Player stepped into river water without safe platform!");
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            Debug.Log($"DEATH CAUSE: VEHICLE COLLISION - Player hit by {collision.gameObject.name}!");
            if (characterAnimController != null)
            {
                characterAnimController.OnPlayerDie();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("River"))
        {
            CheckWaterSafety();
        }
        if (collision.CompareTag("Vehicle"))
        {
            Debug.Log($"DEATH CAUSE: VEHICLE TRIGGER - Player hit by {collision.gameObject.name} (trigger)!");
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
            VehicleMover logMover = collision.GetComponent<VehicleMover>();
            if (logMover != null)
            {
                logVelocity = logMover.direction * logMover.speed;
                onLog = true;
            }
            CheckWaterSafety();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
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
            onLog = false;
            logVelocity = Vector3.zero;
        }
    }
    
        void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    void PlayDeathSound()
    {
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }
    }
}
