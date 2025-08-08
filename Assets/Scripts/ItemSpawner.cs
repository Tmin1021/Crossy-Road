using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject ItemPrefab;
    void Start()
    {
        if (GameObject.Find("Player2"))
        {
            return;
        }
        int r = Random.Range(0, 9);
        if (r == 5)
        {
            SpawnItem();
        }  
        // SpawnItem();
    }
    void SpawnItem()
    {
        
        float rawX = Mathf.Round(Random.Range(-10f, 10f));
        float ItemY = transform.position.y;
        float ItemX = rawX;

        Vector3 spawnPos = new Vector3(ItemX, ItemY, 0f);
        GameObject Item = Instantiate(ItemPrefab, spawnPos, Quaternion.identity);
        Item.transform.SetParent(transform);
    }
}
