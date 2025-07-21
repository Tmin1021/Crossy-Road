using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LightSpawner : MonoBehaviour
{
    public GameObject lightPrefab;
    private GameObject lightSpawned;

    void Start()
    {
        Vector3 spawnPos = new Vector3(0f, transform.position.y, 0f);
        lightSpawned = Instantiate(lightPrefab, spawnPos, Quaternion.identity);
        lightSpawned.transform.SetParent(transform);

        UpdateLightOrder();
    }

    void Update()
    {
        UpdateLightOrder();
    }

    private void UpdateLightOrder()
    {
        int baseOrder = 0;

        SpriteRenderer laneRenderer = GetComponentInParent<SpriteRenderer>();
        if (laneRenderer != null)
        {
            baseOrder = laneRenderer.sortingOrder;
        }

        if (lightSpawned != null)
        {
            SpriteRenderer lightRenderer = lightSpawned.GetComponent<SpriteRenderer>();
            if (lightRenderer != null)
            {
                lightRenderer.sortingOrder = baseOrder + 9;
                Debug.Log("Lightttt");
            }
        }
    }
}
