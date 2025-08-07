using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public int numberOfTrees = 5;
    public bool isRiver = false;
    public bool startLanes = false;
    private int playerId;
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

        playerId = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
    }

    private void SpawnTrees()
    {
        HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
        int spawned = 0;
        int maxAttempts = 100;

        while (spawned < numberOfTrees && maxAttempts > 0)
        {
            maxAttempts--;

            int randomIndex = Random.Range(0, treePrefabs.Length);
            float baseX = Mathf.Round(Random.Range(-11f, 11f));
            float baseY = transform.position.y;

            // Force small tree if position is problematic
            if ((randomIndex == 1 || randomIndex == 2) && (baseX == 0f || baseX == 1f))
            {
                randomIndex = 0;
            }

            // Tree offset & occupied size (in grid units)
            Vector3 offset = Vector3.zero;
            Vector2Int[] occupiedOffsets;

            switch (randomIndex)
            {
                case 0: // Small tree
                    offset = new Vector3(0.25f, 0.25f, 0f);
                    occupiedOffsets = new Vector2Int[] { new Vector2Int(0, 0) };
                    break;
                case 1: // Medium tree
                    offset = new Vector3(0.5f, 0.6f, 0f);
                    occupiedOffsets = new Vector2Int[] {
                        new Vector2Int(0, 0), new Vector2Int(1, 0)
                    };
                    break;
                case 2: // Big tree
                    offset = new Vector3(-0.55f, 0.9f, 0f);
                    occupiedOffsets = new Vector2Int[] {
                        new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1)
                    };
                    break;
                case 3: // Rock
                    offset = new Vector3(-0.9f, 0.25f, 0f);
                    occupiedOffsets = new Vector2Int[] {
                        new Vector2Int(0, 0)
                    };
                    break;
                case 4: // Rock 2 snow lane
                offset = new Vector3(0f, 0.22f, 0f);
                occupiedOffsets = new Vector2Int[] {
                    new Vector2Int(0, 0)
                };
                break;
                default:
                    occupiedOffsets = new Vector2Int[] { new Vector2Int(0, 0) };
                    break;
            }

            // Calculate base grid position
            Vector2Int baseGrid = new Vector2Int(
                Mathf.RoundToInt(baseX * 2), // using *2 to get more precise grid
                Mathf.RoundToInt(baseY * 2)
            );

            // Check if any of the occupied tiles are already used
            bool overlaps = false;
            foreach (var offsetGrid in occupiedOffsets)
            {
                Vector2Int occupied = baseGrid + offsetGrid;
                if (occupiedCells.Contains(occupied))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps) continue;

            // Mark all occupied cells
            foreach (var offsetGrid in occupiedOffsets)
            {
                occupiedCells.Add(baseGrid + offsetGrid);
            }

            // Calculate spawn position
            Vector3 spawnPos = new Vector3(baseX, baseY, 0f) + offset;

            GameObject tree = Instantiate(treePrefabs[randomIndex], spawnPos, Quaternion.identity);
            tree.transform.SetParent(transform);
            spawned++;
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
