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
        if (CompareTag("Coin"))
        {
            GetComponent<SpriteRenderer>().sortingOrder = 100;
            return;
        }
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 2;
    }
}
