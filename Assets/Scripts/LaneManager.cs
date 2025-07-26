using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public GameObject[] lanePrefabs;
    public int numberOfLanes = 15;
    public float laneWidth = 1.0f;
    // public float laneLength = 20.0f;
    private List<GameObject> activeLanes = new List<GameObject>();
    private float lastSpawnY;
    private Transform player;
    
    void Start()
    {
        // Find the spawned player(s) - use Player1 as reference for lane spawning
        StartCoroutine(FindPlayer());
    }
    
    IEnumerator FindPlayer()
    {
        // Wait a frame to let MultiplayerManager spawn the players
        yield return null;
        
        GameObject playerObj = GameObject.Find("Player1");
        if (playerObj == null)
        {
            // Fallback: find any object with PlayerMovement component
            PlayerMovement pm = FindObjectOfType<PlayerMovement>();
            if (pm != null)
            {
                playerObj = pm.gameObject;
            }
        }
        
        if (playerObj != null)
        {
            player = playerObj.transform;
            InitializeLanes();
        }
        else
        {
            Debug.LogError("No player found! Make sure MultiplayerManager spawns players.");
        }
    }
    
    void InitializeLanes()
    {
        lastSpawnY = player.position.y - 2 * laneWidth;
        for (int i = 0; i < numberOfLanes; i++)
        {
            if (i < 3)
            {
                SpawnGrassLane();
            }
            else
            {
                SpawnLane();
            }
        }
        UpdateLaneSorting();
    }
    
    void Update()
    {
        if (player == null) return; // Wait until player is found
        
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
        GameObject lanePrefab = lanePrefabs[randomIndex];
        if (activeLanes.Count > 0)
        {
            if (activeLanes[activeLanes.Count - 1].name != "GrassLane(Clone)"
                && activeLanes[activeLanes.Count - 1].name != "GrassLaneLight(Clone)"
                && randomIndex == 1)
            {
                lanePrefab = lanePrefabs[0];
            }
            else if (randomIndex == 0 && (activeLanes[activeLanes.Count - 1].name == "GrassLaneLight(Clone)" || activeLanes[activeLanes.Count - 1].name == "GrassLane(Clone)"))
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
        if (activeLanes.Count > 0)
        {
            if (activeLanes[activeLanes.Count - 1].name != "GrassLane(Clone)"
                && activeLanes[activeLanes.Count - 1].name != "GrassLaneLight(Clone)"
                && randomIndex == 1)
            {
                lanePrefab = lanePrefabs[0];
            }
            else if (randomIndex == 0 && (activeLanes[activeLanes.Count - 1].name == "GrassLaneLight(Clone)" || activeLanes[activeLanes.Count - 1].name == "GrassLane(Clone)"))
            {
                lanePrefab = lanePrefabs[1];
            }
        }
        Vector3 spawnPosition = new Vector3(0, lastSpawnY + laneWidth, 0);
        GameObject newLane = Instantiate(lanePrefab, spawnPosition, Quaternion.identity);
        newLane.transform.SetParent(transform);
        activeLanes.Add(newLane);
        // Debug.Log("Spawned lane: " + newLane.name);
        // Debug.Log("Last lane: " + activeLanes[activeLanes.Count - 1].name);
        lastSpawnY += laneWidth;
    }

    private void DestroyOldestLane()
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
                sr.sortingOrder = activeLanes.Count - i;
            }
        }
    }
}
