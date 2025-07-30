using UnityEngine;

public class LogMover : MonoBehaviour
{
    public float speed;
    public Vector3 direction;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && direction == Vector3.left)
        {
            sr.flipX = true;
        }
    }
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if ((direction.x > 0 && transform.position.x > 12f) || (direction.x < 0 && transform.position.x < -12f))
        {
            Destroy(gameObject);
        }

        UpdateLogSortingOrder();
    }

    void UpdateLogSortingOrder()
    {
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    float GetSpeed()
    {
        return speed;
    }
}
