using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeOrder : MonoBehaviour
{
    void Start()
    {
        UpdateTreeSortingOrder();
    }

    void Update()
    {
        UpdateTreeSortingOrder();
    }

    void UpdateTreeSortingOrder()
    {
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
