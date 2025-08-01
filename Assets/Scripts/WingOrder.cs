using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingOrder : MonoBehaviour
{
    void Start()
    {
        UpdateWingSortingOrder();
    }

    void Update()
    {
        UpdateWingSortingOrder();
    }

    void UpdateWingSortingOrder()
    {
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
