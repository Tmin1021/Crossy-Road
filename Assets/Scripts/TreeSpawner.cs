using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public int numberOfTrees = 5;
    private int baseOrder = 0;
    public bool isRiver = false;
    public bool startLanes = false;
    void Start()
    {
        if (!startLanes)
        {
            if (!isRiver)
            {
                SpawnTrees();
            }
            else SpawnLilies();
        }
    }

    private void SpawnTrees()
    {
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        int attempts = 0; // prevent infinite loops
        int maxAttempts = 20;

        int spawned = 0;

        while (spawned < numberOfTrees && attempts < maxAttempts)
        {
            int randomIndex = Random.Range(0, treePrefabs.Length);
            float rawX = Mathf.Round(Random.Range(-11f, 11f));
            float treeY = transform.position.y;
            float treeX = rawX;
            if ((randomIndex == 1 || randomIndex == 2) && rawX == 1f)
            {
                randomIndex = 0;
            }

            switch (randomIndex)
            {
                case 0: treeY += 0.25f; treeX += 0.25f; break; // small 
                case 1: treeY += 0.6f; treeX += 0.5f; break; // medium
                case 2: treeY += 0.9f; treeX -= 0.55f; break; // big
                case 3: treeY += 0.25f; treeX -= 0.9f; break; // rock
            }

            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(treeX * 10), Mathf.RoundToInt(treeY * 10));

            if (usedPositions.Contains(gridPos))
            {
                attempts++;
                continue; // position taken, try again
            }

            usedPositions.Add(gridPos);
            attempts = 0; // reset attempts on successful spawn
            spawned++;

            Vector3 spawnPos = new Vector3(treeX, treeY, 0f);
            GameObject tree = Instantiate(treePrefabs[randomIndex], spawnPos, Quaternion.identity);
            tree.transform.SetParent(transform);
        }
    }

    void SpawnLilies()
    {
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
        numberOfTrees = 8;
        int attempts = 0; // prevent infinite loops
        int maxAttempts = 20;

        int spawned = 0;

        while (spawned < numberOfTrees && attempts < maxAttempts)
        {
            float rawX = Mathf.Round(Random.Range(-12f, 12f));
            float treeY = transform.position.y;
            float treeX = rawX;

            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(treeX * 10), Mathf.RoundToInt(treeY * 10));

            if (usedPositions.Contains(gridPos))
            {
                attempts++;
                continue; // position taken, try again
            }

            usedPositions.Add(gridPos);
            attempts = 0; // reset attempts on successful spawn
            spawned++;

            Vector3 spawnPos = new Vector3(treeX, treeY, 0f);
            GameObject lily = Instantiate(treePrefabs[0], spawnPos, Quaternion.identity);
            lily.transform.SetParent(transform);
        }
    }
}
