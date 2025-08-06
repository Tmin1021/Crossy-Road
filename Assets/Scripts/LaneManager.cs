using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;
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
    CharacterManager characterManager;
    public int playerId;

    void Start()
    {
        InitializeLanes();
        GameObject playerObj = GameObject.Find("Player1");
        player = playerObj?.transform;
        playerId = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
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
        if (playerId == 0)
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
        if (playerId == 0)
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
            if (playerId == 0)
            {
                if (activeLanes[numLanes - 1].name != "SnowGrassLane(Clone)"
                && activeLanes[numLanes - 1].name != "SnowGrassLaneLight(Clone)"
                && randomIndex == 1)
                {
                    lanePrefab = snowLanePrefabs[0];
                }
                else if (randomIndex == 0
                && (activeLanes[numLanes - 1].name == "SnowGrassLaneLight(Clone)"
                || activeLanes[numLanes - 1].name == "SnowGrassLane(Clone)"))
                {
                    lanePrefab = snowLanePrefabs[1];
                }
            }
            else if (playerId != 0)
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
    } 

    public void DestroyOldestLane()
    {
        GameObject oldestLane = activeLanes[0];
        if (activeLanes.Count > 13)
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
