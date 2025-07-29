using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour
{
    public float baseSpeed = 2.0f;
    public Vector3 direction;
    
    private float currentSpeedMultiplier = 1.0f;
    
    public float speed 
    { 
        get { return baseSpeed * currentSpeedMultiplier; } 
    }
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && direction == Vector3.left)
        {
            sr.flipX = true;
        }
        
        // Register with GameModeManager if it exists
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.RegisterNewVehicle(this);
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
    
    public void ApplySpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
        Debug.Log($"Vehicle speed multiplier set to: {multiplier}");
    }
}
