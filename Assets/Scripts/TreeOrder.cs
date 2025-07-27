using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeOrder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateTreeSortingOrder();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTreeSortingOrder();
    }

    void UpdateTreeSortingOrder()
    {
        GameObject parent = transform.parent.gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder + 2;
    }
}
