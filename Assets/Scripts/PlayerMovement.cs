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

    void Start()
    {
        animator = GetComponent<Animator>();
        characterAnimController = GetComponent<CharacterAnimationController>();
    }

    void Update()
    {
        // If we don't have the CharacterAnimationController yet, try to find it
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
}
