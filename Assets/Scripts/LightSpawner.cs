using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LightSpawner : MonoBehaviour
{
    public GameObject lightPrefab;
    public GameObject lightSpawned;

    [Range(0f, 1f)]
    public float lightChance = 0.3f;

    void Start()
    {
        // if (Random.value > lightChance)
        // {
        //     return;
        // }

        Vector3 spawnPos = new Vector3(0f, transform.position.y + 0.03f, 0f);
        lightSpawned = Instantiate(lightPrefab, spawnPos, Quaternion.identity);
        lightSpawned.transform.SetParent(transform);
    }
}
