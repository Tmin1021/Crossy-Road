using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public int numberOfTrees = 5;
    private int baseOrder = 0;
    void Start()
    {
        SpawnTrees();
    }

    void Update()
    {
        UpdateTreeSorting();
    }

    private void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            int randomIndex = Random.Range(0, treePrefabs.Length);
            float treeY = transform.position.y;
            switch (randomIndex)
            {
                case 0: // small tree 
                    treeY += 0.25f;
                    break;
                case 1: // medium tree 
                    treeY += 0.6f;
                    break;
                case 2: // big tree 
                    treeY += 0.95f;
                    break;
                case 3: // rock
                    treeY += 0.25f;
                    break;
            }

            Vector3 spawnPos = new Vector3(Random.Range(-12f, 12f), treeY, 0f);
            GameObject tree = Instantiate(treePrefabs[randomIndex], spawnPos, Quaternion.identity);
            tree.transform.SetParent(transform);

            SpriteRenderer laneRenderer = GetComponentInParent<SpriteRenderer>();
            if (laneRenderer != null)
            {
                baseOrder = laneRenderer.sortingOrder;
            }

            SpriteRenderer sr = tree.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = baseOrder + 12; // 12 maybe false in the future when interval set to random
            }
        }
    }

    private void UpdateTreeSorting()
    {
        SpriteRenderer laneRenderer = GetComponentInParent<SpriteRenderer>();
        if (laneRenderer != null)
        {
            baseOrder = laneRenderer.sortingOrder;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = baseOrder + 12;
            }
        }
    } 
}
