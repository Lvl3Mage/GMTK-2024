﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PlantSelector : MonoBehaviour
{
    [SerializeField] Vector2Int[] plantGeneratorOffsets;
    int a;
    public IEnumerator SelectPlant()
    {
        while (false)
        {
            yield return null;
        }
    }
    public PlantGenerator GetPlantGenerator()
    {

        // return (rootPosition) => {
        //     return new HashSet<Vector2Int>();
        // };
        return position => {
            HashSet<Vector2Int> plantPositions = new();
            foreach (Vector2Int offset in plantGeneratorOffsets){
                plantPositions.Add(position + offset);
            }
            return plantPositions;
        };
    }
}

public delegate HashSet<Vector2Int> PlantGenerator(Vector2Int rootPosition);