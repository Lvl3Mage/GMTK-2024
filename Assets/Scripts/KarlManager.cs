using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarlManager : MonoBehaviour
{
    [SerializeField]MapRenderer map;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            map.FillBounds();
            map.renderWorldGrid();
        }
    }
}
