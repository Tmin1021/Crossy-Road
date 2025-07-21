using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour
{
    public float speed = 2.0f;
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
        LightSpawner lightSpawner = GetComponentInParent<LightSpawner>();
        bool canMove = true;

        if (lightSpawner != null)
        {
            Debug.Log("Checking traffic light state");
            TrafficLightController light = lightSpawner.lightSpawned.GetComponent<TrafficLightController>();
            if (light != null && !light.IsGreen)
            {
                canMove = false;
            }
        }

        if (canMove)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        if ((direction.x > 0 && transform.position.x > 12f) || (direction.x < 0 && transform.position.x < -12f))
        {
            Destroy(gameObject);
        }
    }
}
