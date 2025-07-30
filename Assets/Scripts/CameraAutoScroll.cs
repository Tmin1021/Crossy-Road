using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraAutoScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f;   
    public float difficultyIncreaseRate = 0f;  
    private float elapsedTime = 0f;

    public Text textObject;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 1f)
        {
            scrollSpeed += difficultyIncreaseRate;
            elapsedTime = 0f;
        }

        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
    }

    public void MoveUpOneLane()
    {
        transform.position += Vector3.up;
    }
}
