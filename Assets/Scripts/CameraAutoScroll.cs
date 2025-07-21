using System.Collections;
using UnityEngine;

public class CameraAutoScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f; 
    // private bool isScrolling = false;

    // Start the camera scroll when called
    // public void StartCameraScroll()
    // {
    //     isScrolling = true;
    // }

    void Update()
    {
        //if (isScrolling)
        //{
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
        //}
    }
}
