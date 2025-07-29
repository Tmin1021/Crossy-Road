using System.Collections;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    private float baseSpawnInterval;
    private float currentDelayMultiplier = 1.0f;
    public bool spawnRight = false;
    private float vehicleSpeed;
    private float timer;
    public bool ifRailWay = false;
    // private bool hasTrainSpawned = false; // futher update for traffic ligh
    private bool isRiver = false;
    
    private float spawnInterval 
    { 
        get { return baseSpawnInterval * currentDelayMultiplier; } 
    }

    void Awake()
    {
        if (gameObject.CompareTag("River"))
        {
            isRiver = true;
        }
    }

    void Start()
    {
        if (!ifRailWay)
        {
            baseSpawnInterval = Random.Range(0f, 2f) + 1.5f;
            vehicleSpeed = Random.Range(0f, 2f) + 1f;
            spawnRight = Random.Range(0, 2) == 0;

            SpawnRandomVehicle();
        }
        else
        {
            baseSpawnInterval = 3f;
            vehicleSpeed = 100f; 
            spawnRight = Random.Range(0, 2) == 0;
        }
        
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.RegisterNewSpawner(this);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!isRiver)
        {
            LightSpawner lightSpawner = GetComponent<LightSpawner>();
            if (lightSpawner != null && lightSpawner.lightSpawned != null)
            {
                TrafficLightController light = lightSpawner.lightSpawned.GetComponent<TrafficLightController>();
                if (light != null && !light.IsGreen)
                {
                    Debug.Log("OK");
                    timer = 0f;
                    return; 
                }
            }
        }

        // if (ifRailWay)
        // {
        //     if (!hasTrainSpawned)
        //     {
        //         SpawnRandomVehicle();
        //         hasTrainSpawned = true;
        //     }
        //     return;
        // }

        if (timer >= spawnInterval)
        {
            SpawnRandomVehicle();
            timer = 0f;
        }
    }


    void SpawnRandomVehicle()
    {
        int randomIndex = Random.Range(0, vehiclePrefabs.Length);
        Vector3 spawnPos;

        Vector3 direction;
        if (spawnRight)
        {
            spawnPos = new Vector3(-12f, transform.position.y + 0.1f, 0f);
            direction = Vector3.right;
        }
        else
        {
            spawnPos = new Vector3(12f, transform.position.y + 0.1f, 0f);
            direction = Vector3.left;
        }

        GameObject vehicle = Instantiate(vehiclePrefabs[randomIndex], spawnPos, Quaternion.identity);
        if (vehicle.CompareTag("Log"))
        {
            Vector3 pos = vehicle.transform.position;
            pos.y -= 0.155f;
            vehicle.transform.position = pos;
        }
        vehicle.transform.SetParent(transform);
        VehicleMover mover = vehicle.GetComponent<VehicleMover>();
        if (mover != null)
        {
            mover.direction = direction;
        }

        UpdateVehicleSorting(vehicle);
    }

    private void UpdateVehicleSorting(GameObject vehicle)
    {
        int baseOrder = 0;

        SpriteRenderer laneRenderer = GetComponentInParent<SpriteRenderer>();
        if (laneRenderer != null)
        {
            baseOrder = laneRenderer.sortingOrder;
        }

        SpriteRenderer sr = vehicle.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = baseOrder + 10; 
        }
    }
    
    public void ApplyDelayMultiplier(float multiplier)
    {
        currentDelayMultiplier = multiplier;
        Debug.Log($"Vehicle spawner delay multiplier set to: {multiplier}");
    }
}
