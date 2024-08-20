using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueGameObject : MonoBehaviour
{
    public static HashSet<string> uniqueGameObjects = new();
    [SerializeField] string uniqueObjectName;

    void Awake()
    {
        if (!uniqueGameObjects.Add(uniqueObjectName))
        {
            Destroy(gameObject);
        }
    }
}
