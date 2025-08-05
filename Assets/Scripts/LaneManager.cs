using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaneManager : MonoBehaviour
{
    public GameObject[] lanePrefabs;
    public int numberOfLanes = 15;
    public float laneWidth = 1.0f;
    private List<GameObject> activeLanes = new List<GameObject>();
    public float lastSpawnY;
    public Transform player;
    public int numberOfGrassLanes = 4;

    void Start()
    {
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

    private void SpawnGrassLane()
    {
        int randomIndex = Random.Range(0, 2);
        int numLanes = activeLanes.Count;
        GameObject lanePrefab = lanePrefabs[randomIndex];
        if (numLanes> 0)
        {
            if (activeLanes[numLanes- 1].name != "GrassLane(Clone)"
                && activeLanes[numLanes- 1].name != "GrassLaneLight(Clone)"
                && randomIndex == 1)
            {
                lanePrefab = lanePrefabs[0];
            }
            else if (randomIndex == 0
            && (activeLanes[numLanes - 1].name == "GrassLaneLight(Clone)"
            || activeLanes[numLanes - 1].name == "GrassLane(Clone)"))
            {
                lanePrefab = lanePrefabs[1];
            }
        }
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
        GameObject lanePrefab = lanePrefabs[randomIndex];
        int numLanes = activeLanes.Count;
        if (numLanes > 0)
        {
            if (activeLanes[numLanes - 1].name != "GrassLane(Clone)"
                && activeLanes[numLanes - 1].name != "GrassLaneLight(Clone)"
                && randomIndex == 1)
            {
                lanePrefab = lanePrefabs[0];
            }
            else if (randomIndex == 0
            && (activeLanes[numLanes - 1].name == "GrassLaneLight(Clone)"
            || activeLanes[numLanes - 1].name == "GrassLane(Clone)"))
            {
                lanePrefab = lanePrefabs[1];
            }
            else if (activeLanes[numLanes - 1].name == "RiverLaneLog(Clone)"
            && randomIndex == 3)
            {
                bool tmp = activeLanes[numLanes - 1].GetComponent<LogSpawner>().spawnRight;
                bool tmp1 = false;
                if (!tmp) tmp1 = true;
                lanePrefab.GetComponent<LogSpawner>().flag = true;
                lanePrefab.GetComponent<LogSpawner>().spawnRight = tmp1;
            }
            else if (activeLanes[numLanes - 1].name == "RiverLaneLog(Clone)"
            && activeLanes[numLanes - 2].name == "RiverLaneLog(Clone)")
            {
                if (randomIndex == 3)
                {
                    lanePrefab = lanePrefabs[0];
                }
            }
        }
        Vector3 spawnPosition = new Vector3(0, lastSpawnY + laneWidth, 0);
        GameObject newLane = Instantiate(lanePrefab, spawnPosition, Quaternion.identity);
        newLane.transform.SetParent(transform);
        activeLanes.Add(newLane);
        lastSpawnY += laneWidth;
    }

    public void DestroyOldestLane()
    {
        GameObject oldestLane = activeLanes[0];
        if (activeLanes.Count> 13)
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
