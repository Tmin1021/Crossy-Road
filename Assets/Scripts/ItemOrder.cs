using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOrder : MonoBehaviour
{
    void Start()
    {
        UpdateItemSortingOrder();
    }

    void Update()
    {
        UpdateItemSortingOrder();
    }

    void UpdateItemSortingOrder()
    {
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
