using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour
{
    public float speed = 5.0f;
    public Vector3 direction;
    private float offScreenX = 12.0f;
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

        if ((direction.x > 0 && transform.position.x > offScreenX) || (direction.x < 0 && transform.position.x < -offScreenX))
        {
            // Debug.Log("Vehicle out of bounds, destroying: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}
