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
    // private bool hasTrainSpawned = false; // futher update for traffic ligh
    private bool isRiver = false;

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
            spawnInterval = Random.Range(0f, 2f) + 1.5f;
            vehicleSpeed = Random.Range(0f, 2f) + 1f;
            spawnRight = Random.Range(0, 2) == 0;

            SpawnRandomVehicle();
        }
        else {
            spawnInterval = 3f;
            spawnRight = Random.Range(0, 2) == 0;
        }  
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!isRiver && ifRailWay)
        {
            LightSpawner lightSpawner = GetComponent<LightSpawner>();
            if (lightSpawner != null && lightSpawner.lightSpawned != null)
            {
                TrafficLightController light = lightSpawner.lightSpawned.GetComponent<TrafficLightController>();
                if (light != null && !light.IsGreen)
                {
                    // Debug.Log("OK");
                    timer = 0f;
                    return; // Red light: don't spawn
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

        // Update later -> Direction 
        VehicleMover mover = vehicle.GetComponent<VehicleMover>();
        if (mover != null)
        {
            // mover.speed = vehicleSpeed;
            mover.direction = direction;
        }
    }
}
