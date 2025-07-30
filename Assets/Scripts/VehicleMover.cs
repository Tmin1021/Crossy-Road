using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour
{
    public float baseSpeed = 2.0f;
    public Vector3 direction;
    private float stopX;
    private bool isBlockedByVehicle = false;
    private bool hasPassedStopLine = false;


    
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

        stopX = direction == Vector3.right ? -1f : 1f;
        
        // Register with GameModeManager if it exists
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.RegisterNewVehicle(this);
        }
    }
    void Update()
    {
        LightSpawner lightSpawner = GetComponentInParent<LightSpawner>();
        VehicleSpawner vehicleSpawner = GetComponentInParent<VehicleSpawner>();
        bool canMove = true;

        CheckIfBlockedByOtherVehicle();

        TrafficLightController light = null;
        if (lightSpawner != null && lightSpawner.lightSpawned != null)
        {
            light = lightSpawner.lightSpawned.GetComponent<TrafficLightController>();
        }

        if (!hasPassedStopLine && light != null && !light.IsGreen && !lightSpawner.lightSpawned.CompareTag("RailLight"))
        {
            if (direction == Vector3.right && transform.position.x >= stopX)
            {
                canMove = false;
            }
            else if (direction == Vector3.left && transform.position.x <= stopX)
            {
                canMove = false;
            }
        }

        if (!hasPassedStopLine && light != null && light.IsGreen)
        {
            if (direction == Vector3.right && transform.position.x >= stopX)
            {
                hasPassedStopLine = true;
            }
            else if (direction == Vector3.left && transform.position.x <= stopX)
            {
                hasPassedStopLine = true;
            }
        }

        if (canMove && !isBlockedByVehicle)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        if ((direction.x > 0 && transform.position.x > 12f) || (direction.x < 0 && transform.position.x < -12f))
        {
            Destroy(gameObject);
        }

        UpdateVehicleSortingOrder();
    }

    void UpdateVehicleSortingOrder()
    {
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    void CheckIfBlockedByOtherVehicle()
    {
        isBlockedByVehicle = false;

        Vector2 boxSize = new Vector2(1.5f, 0.5f);
        float castDistance = 1.5f;

        Vector2 origin = transform.position;
        Vector2 castDirection = new Vector2(direction.x, 0);

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Vehicle"));
        filter.useTriggers = true;

        RaycastHit2D[] hits = new RaycastHit2D[5];
        int hitCount = Physics2D.BoxCast(origin, boxSize, 0f, castDirection, filter, hits, castDistance);

        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject != this.gameObject)
            {
                isBlockedByVehicle = true;
                break;
            }
        }

        // Visual debug
        // Debug.DrawRay(origin, castDirection * castDistance, Color.yellow);
        // DebugDrawBoxCast(origin, boxSize, castDirection, castDistance);
    }

    // void DebugDrawBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
    // {
    //     Vector2 end = origin + direction * distance;

    //     Vector2 topLeft = new Vector2(origin.x - size.x / 2, origin.y + size.y / 2);
    //     Vector2 topRight = new Vector2(origin.x + size.x / 2, origin.y + size.y / 2);
    //     Vector2 bottomLeft = new Vector2(origin.x - size.x / 2, origin.y - size.y / 2);
    //     Vector2 bottomRight = new Vector2(origin.x + size.x / 2, origin.y - size.y / 2);

    //     Vector2 topLeftEnd = topLeft + direction * distance;
    //     Vector2 topRightEnd = topRight + direction * distance;
    //     Vector2 bottomLeftEnd = bottomLeft + direction * distance;
    //     Vector2 bottomRightEnd = bottomRight + direction * distance;

    //     Debug.DrawLine(topLeft, topRight, Color.cyan);
    //     Debug.DrawLine(topRight, bottomRight, Color.cyan);
    //     Debug.DrawLine(bottomRight, bottomLeft, Color.cyan);
    //     Debug.DrawLine(bottomLeft, topLeft, Color.cyan);

    //     Debug.DrawLine(topLeftEnd, topRightEnd, Color.cyan);
    //     Debug.DrawLine(topRightEnd, bottomRightEnd, Color.cyan);
    //     Debug.DrawLine(bottomRightEnd, bottomLeftEnd, Color.cyan);
    //     Debug.DrawLine(bottomLeftEnd, topLeftEnd, Color.cyan);

    //     Debug.DrawLine(topLeft, topLeftEnd, Color.cyan);
    //     Debug.DrawLine(topRight, topRightEnd, Color.cyan);
    //     Debug.DrawLine(bottomLeft, bottomLeftEnd, Color.cyan);
    //     Debug.DrawLine(bottomRight, bottomRightEnd, Color.cyan);
    // }

    
    public void ApplySpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
        Debug.Log($"Vehicle speed multiplier set to: {multiplier}");
    }
}
