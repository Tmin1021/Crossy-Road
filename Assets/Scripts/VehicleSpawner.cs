using System.Collections;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    private float spawnInterval;
    public bool spawnRight = false;
    private float vehicleSpeed;
    private float timer;
    public bool ifRailWay = false;
    private bool hasTrainSpawned = false; // futher update for traffic ligh

    void Start()
    {
        spawnInterval = Random.Range(0f, 2f) + 2f;
        vehicleSpeed = Random.Range(0f, 2f) + 2f;
        spawnRight = Random.Range(0, 2) == 0;
    }

    void Update()
    {
        if (ifRailWay)
        {
            // timer = 0f + Time.deltaTime;
            if (!hasTrainSpawned)
            {
                SpawnRandomVehicle();
                hasTrainSpawned = true;
            }

            return;
        }
        timer += Time.deltaTime;
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
        vehicle.transform.SetParent(transform);

        // Update later -> Direction 
        VehicleMover mover = vehicle.GetComponent<VehicleMover>();
        if (mover != null)
        {
            mover.speed = vehicleSpeed;
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
            sr.sortingOrder = baseOrder + 10; // 12 maybe false in the future when interval set to random
        }
    }
}
