using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f;
    private bool isMoving = false;
    private Animator animator;
    public LayerMask obstacleLayer;
    private Vector3 logVelocity = Vector3.zero;
    private bool onLog = false;
    public KeyCode upKey, downKey, leftKey, rightKey;
    // s
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(upKey))
            {
                StartCoroutine(Move(Vector3.up, "Up"));
                // laneManager.SpawnLane();
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

        if (transform.position.y < cameraBottomY)
        {
            Debug.Log("Player has fallen off the screen!");
        }

        if (onLog)
        {
            transform.position += logVelocity * Time.deltaTime;
        }
    }

    IEnumerator Move(Vector3 direction, string trigger)
    {
        isMoving = true;

        resetTriggers();

        animator.SetTrigger(trigger);
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * moveDistance;
        float elapsed = 0f;
        float duration = 0.1f;

        Collider2D hit = Physics2D.OverlapCircle(endPos, 0.05f, obstacleLayer);
        if (hit != null)
        {
            // Debug.Log("Blocked by obstacle: " + hit.gameObject.name);
            isMoving = false;
            yield break; // Stop the coroutine early
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
        isMoving = false;
    }

    void resetTriggers()
    {
        animator.ResetTrigger("Up");
        animator.ResetTrigger("Down");
        animator.ResetTrigger("Left");
        animator.ResetTrigger("Right");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            Debug.Log("Player hit by vehicle!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("River"))
        {
            Collider2D lilyPad = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("LilyPad"));

            if (lilyPad != null)
            {
                Debug.Log("Player landed on a lily pad. Safe!");
            }
            else
            {
                // resetTriggers();
                // animator.SetTrigger("Die");
                // gameObject.GetComponent<PlayerMovement>().enabled = false;
                Debug.Log("Player fell into the river and died!");
            }
        }
        // if (collision.CompareTag("LilyPad"))
        // {
        //     Debug.Log("Player landed on a lily pad. Safe!");
        // }
        if (collision.CompareTag("Vehicle"))
        {
            resetTriggers();
            animator.SetTrigger("Die");
            gameObject.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Hit by vehicle");
        }
        if (collision.CompareTag("Log"))
        {
            VehicleMover logMover = collision.GetComponent<VehicleMover>();
            if (logMover != null)
            {
                logVelocity = logMover.direction * logMover.speed;
                onLog = true;
            }

            Debug.Log("Player on a log");
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
