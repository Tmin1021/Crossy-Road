using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f;
    private bool isMoving = false;
    private Animator animator;
    public LayerMask obstacleLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Move(Vector3.up, "Up"));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(Move(Vector3.down, "Down"));
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Move(Vector3.left, "Left"));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Move(Vector3.right, "Right"));
            }
        }

        float cameraBottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;

        if (transform.position.y < cameraBottomY)
        {
            Debug.Log("Player has fallen off the screen!");
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("River"))
        {
            Debug.Log("Player fallen into the river!");
        }
        if (other.CompareTag("Vehicle"))
        {
            // animator.SetTrigger("Die");
            // gameObject.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Hit by vehicle");
        }
    }

}
