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
                angle = 10f;
                break;
            case 2:
                angle = 20f;
                break;
            case 3:
                angle = 30f;
                break;

        }
        transform.Rotate(0f, 0f, angle);
        randomIndex = Random.Range(0, 3);
    }

    void Update()
    {
        transform.Rotate(0f, 0f, 0.2f);
    }
}
