using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingSpawner : MonoBehaviour
{
    public GameObject wingPrefab;
    void Start()
    {
        int r = Random.Range(0, 3);
        if (r == 0)
        {
            SpawnWing();
        }  
    }
    void SpawnWing()
    {
        
        float rawX = Mathf.Round(Random.Range(-10f, 10f));
        float wingY = transform.position.y;
        float wingX = rawX;

        Vector3 spawnPos = new Vector3(wingX, wingY, 0f);
        GameObject wing = Instantiate(wingPrefab, spawnPos, Quaternion.identity);
        wing.transform.SetParent(transform);
    }
}
