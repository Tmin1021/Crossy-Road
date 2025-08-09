using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaneManager : MonoBehaviour
{
    public GameObject[] lanePrefabs;
    public GameObject[] snowLanePrefabs;
    public int numberOfLanes = 15;
    public float laneWidth = 1.0f;
    private List<GameObject> activeLanes = new List<GameObject>();
    private float lastSpawnY;
    public Transform player;
    private int numberOfGrassLanes = 4;
    public int playerId;

    void Start()
    {
        playerId = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        InitializeLanes();
        GameObject playerObj = GameObject.Find("Player1");
        player = playerObj?.transform;
    }

    void Update()
    {
        float cameraTopY = Camera.main.transform.position.y + Camera.main.orthographicSize;

        // When player moves forward, spawn more
        if (cameraTopY > lastSpawnY)
        {
            SpawnLane();
            DestroyOldestLane();
            UpdateLaneSorting();
        }
    }

    public float getLastSpawnY()
    {
        return lastSpawnY;
    }

    private void SpawnGrassLane()
    {
        GameObject lanePrefab;
        if (playerId == 0 || playerId == 4)
        {
            lanePrefab = snowLanePrefabs[1];
        }
        else lanePrefab = lanePrefabs[1];
        
        Vector3 spawnPosition = new Vector3(0, lastSpawnY + laneWidth, 0);
        GameObject newLane = Instantiate(lanePrefab, spawnPosition, Quaternion.identity);
        newLane.transform.SetParent(transform);
        activeLanes.Add(newLane);
        lastSpawnY += laneWidth;
        TreeSpawner treeSpawner = newLane.GetComponent<TreeSpawner>();
        treeSpawner.startLanes = true;
    }

    public void SpawnLane()
    {
        int randomIndex = Random.Range(0, lanePrefabs.Length);
        GameObject lanePrefab;
        if (playerId == 0 || playerId == 4)
        {
            lanePrefab = snowLanePrefabs[randomIndex];
        }
        else lanePrefab = lanePrefabs[randomIndex];

        LanePrefabsCheckCondition(playerId, ref lanePrefab, randomIndex);

        Vector3 spawnPosition = new Vector3(0, lastSpawnY + laneWidth, 0);
        GameObject newLane = Instantiate(lanePrefab, spawnPosition, Quaternion.identity);
        newLane.transform.SetParent(transform);
        activeLanes.Add(newLane);
        lastSpawnY += laneWidth;
    }

    public void LanePrefabsCheckCondition(int playerId, ref GameObject lanePrefab, int randomIndex)
    {
        int numLanes = activeLanes.Count;

        if (numLanes > 0)
        {
            string lastLaneName = activeLanes[numLanes - 1].name;

            // First, handle the log alternation logic
            if (lastLaneName == "RiverLaneLogL(Clone)" && randomIndex == 3)
            {
                lanePrefab = lanePrefabs[4]; // Switch to Right log
                return;
            }
            else if (lastLaneName == "RiverLaneLogR(Clone)" && randomIndex == 4)
            {
                lanePrefab = lanePrefabs[3]; // Switch to Left log
                return;
            }

            // Then handle the snow/grass lane conditions
            if (playerId == 0 || playerId == 4) // Snow player
            {
                if (lastLaneName != "SnowGrassLane(Clone)" && lastLaneName != "SnowGrassLaneLight(Clone)" && randomIndex == 1)
                {
                    lanePrefab = snowLanePrefabs[0];
                }
                else if (randomIndex == 0 && (lastLaneName == "SnowGrassLaneLight(Clone)" || lastLaneName == "SnowGrassLane(Clone)"))
                {
                    lanePrefab = snowLanePrefabs[1];
                }
            }
            else // Regular player
            {
                if (lastLaneName != "GrassLane(Clone)" && lastLaneName != "GrassLaneLight(Clone)" && randomIndex == 1)
                {
                    lanePrefab = lanePrefabs[0];
                }
                else if (randomIndex == 0 && (lastLaneName == "GrassLaneLight(Clone)" || lastLaneName == "GrassLane(Clone)"))
                {
                    lanePrefab = lanePrefabs[1];
                }
            }
        }
    }


    public void DestroyOldestLane()
    {
        GameObject oldestLane = activeLanes[0];
        if (activeLanes.Count > 15)
        {
            activeLanes.RemoveAt(0);
            Destroy(oldestLane);
        }
    }

    private void UpdateLaneSorting()
    {
        for (int i = 0; i < activeLanes.Count; i++)
        {
            SpriteRenderer sr = activeLanes[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = activeLanes.Count- i;
            }
        }
    }
    
    void InitializeLanes()
    {
        float cameraBottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        lastSpawnY = cameraBottomY - laneWidth; // a bit below screen

        for (int i = 0; i < numberOfLanes; i++)
        {
            if (i < numberOfGrassLanes)
            {
                SpawnGrassLane();
                continue;
            }
            SpawnLane();
        }
        UpdateLaneSorting();
    }
}
