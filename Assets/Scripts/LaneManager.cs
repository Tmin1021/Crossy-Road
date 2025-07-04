using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject[] lanePrefabs;
    public int numberOfLanes = 10;
    public float laneWidth = 1.0f;
    // public float laneLength = 20.0f;
    private List<GameObject> activeLanes = new List<GameObject>();
    private float lastSpawnY;
    public Transform player;
    void Start()
    {
        lastSpawnY = player.position.y - 2 * laneWidth;
        for (int i = 0; i < numberOfLanes; i++)
        {
            SpawnLane();
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

    private void SpawnLane()
    {
        int randomIndex = Random.Range(0, lanePrefabs.Length);
        GameObject lanePrefab = lanePrefabs[randomIndex];
        if (activeLanes.Count > 0)
        {
            if (activeLanes[activeLanes.Count - 1].name != "GrassLane(Clone)" && activeLanes[activeLanes.Count - 1].name != "GrassLaneLight(Clone)" && randomIndex == 1)
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
        if (activeLanes.Count > 12)
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
