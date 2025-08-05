using System.Collections;
using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    public GameObject[] logPrefabs;
    private float spawnInterval;
    public bool spawnRight = false;
    public bool flag = false;
    private float logSpeed;
    private float timer;


    void Start()
    {

        spawnInterval = 2f;
        if (!flag)
        {
            spawnRight = Random.Range(0, 2) == 0;
        }

        if (!spawnRight)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        // logSpeed = Random.Range(3f, 5f);

        SpawnRandomlog();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomlog();
            timer = 0f;
        }
    }


    void SpawnRandomlog()
    {
        int randomIndex = Random.Range(0, logPrefabs.Length);
        Vector3 spawnPos;

        Vector3 direction;
        if (spawnRight)
        {
            spawnPos = new Vector3(-12f, transform.position.y + 0.1f, 0f);
            direction = Vector3.right;
        }
        else
        {
            spawnPos = new Vector3(12f, transform.position.y + 0.1f, 0f);
            direction = Vector3.left;
        }

        GameObject log = Instantiate(logPrefabs[randomIndex], spawnPos, Quaternion.identity);
        if (log.CompareTag("Log"))
        {
            Vector3 pos = log.transform.position;
            pos.y -= 0.155f;
            log.transform.position = pos;
        }
        log.transform.SetParent(transform);

        // Update later -> Direction 
        LogMover mover = log.GetComponent<LogMover>();
        if (mover != null)
        {
            // mover.speed = logSpeed;
            mover.direction = direction;
        }
    }
}
