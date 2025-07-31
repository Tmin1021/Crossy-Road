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
    // public float laneLength = 20.0f;
    private List<GameObject> activeLanes = new List<GameObject>();
    public float lastSpawnY;
    public Transform player;
    public int numberOfGrassLanes = 7;
    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"LaneManager: Current scene is '{currentSceneName}'");
        
        if (currentSceneName == "SoloScene")
        {
            Debug.Log("LaneManager: Solo scene detected, initializing lanes immediately");
            // In solo scene, player already exists, so initialize lanes directly
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null)
            {
                playerObj = GameObject.Find("Player");
            }
            if (playerObj == null)
            {
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
                Debug.Log("LaneManager: Successfully initialized lanes for solo scene");
            }
            else
            {
                Debug.LogError("LaneManager: No player found in solo scene!");
            }
        }
        else
        {
            Debug.Log("LaneManager: Multiplayer scene detected, waiting for player spawn");
            StartCoroutine(FindPlayer());
        }
    }
    
    IEnumerator FindPlayer()
    {
        Debug.Log("LaneManager: Starting player search coroutine");
        yield return new WaitForSeconds(0.5f); // Wait a bit longer for multiplayer spawning
        
        GameObject playerObj = GameObject.Find("Player1");
        if (playerObj == null)
        {
            PlayerMovement pm = FindObjectOfType<PlayerMovement>();
            if (pm != null)
            {
                playerObj = pm.gameObject;
                Debug.Log($"LaneManager: Found player via PlayerMovement: {playerObj.name}");
            }
        }
        else
        {
            Debug.Log("LaneManager: Found Player1");
        }
        
        if (playerObj != null)
        {
            player = playerObj.transform;
            InitializeLanes();
            Debug.Log($"LaneManager: Successfully assigned player and initialized lanes for {playerObj.name}");
        }
        else
        {
            Debug.LogError("LaneManager: No player found! Make sure MultiplayerSceneSetup spawns players.");
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
                sr.sortingOrder = activeLanes.Count - i;
            }
        }
    }
}
