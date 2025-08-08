using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLily : MonoBehaviour
{

    private int randomIndex;
    void Start()
    {
        float angle = 0f;
        int random = Random.Range(1, 4);
        switch (random)
        {
            case 0:
                angle = 0f;
                break;
            case 1:
                angle = 5f;
                break;
            case 2:
                angle = 10f;
                break;
            case 3:
                angle = 15f;
                break;

        }
        transform.Rotate(0f, 0f, angle);
        randomIndex = Random.Range(0, 3);
    }

    void Update()
    {
        float angle = 0.3f;
        switch (randomIndex)
        {
            case 0:
                angle = 0.3f;
                break;
            case 1:
                angle = 0.4f;
                break;
            case 2:
                angle = 0.5f;
                break; 
        }
        transform.Rotate(0f, 0f, angle);
    }
}
