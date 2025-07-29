using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    
    [Header("Animation Settings")]
    public float bobAmount = 0.1f;
    public float bobSpeed = 10f;
    
    private Vector3 originalPosition;
    private bool isAnimating = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        originalPosition = transform.position;
    }
    
    void Update()
    {
        if (playerMovement != null && playerMovement.isMoving)
        {
            if (!isAnimating)
            {
                StartBobAnimation();
            }
        }
        else
        {
            if (isAnimating)
            {
                StopBobAnimation();
            }
        }
    }
    
    void StartBobAnimation()
    {
        isAnimating = true;
        StartCoroutine(BobAnimation());
    }
    
    void StopBobAnimation()
    {
        isAnimating = false;
        StopAllCoroutines();
        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
    }
    
    System.Collections.IEnumerator BobAnimation()
    {
        float time = 0f;
        Vector3 startPos = transform.position;
        
        while (isAnimating)
        {
            float yOffset = Mathf.Sin(time * bobSpeed) * bobAmount;
            transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
