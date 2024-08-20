using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

class PlantSelector : MonoBehaviour
{
    public int AveragePlantSize = 6;
    public int PlantMinSize = 3;
    public int PlantMaxSize = 9;
    [Range(0, 1)][SerializeField] float RNGManipulation = 0.5f;
    int currentTax = 0;

    public IEnumerator SelectPlant()
    {
        while (false)
        {
            yield return null;
        }
    }

    PlantGenerator CreatePlantShape()
    {
        HashSet<Vector2Int> plantPositions = new() { Vector2Int.zero }; //Origin position by default

        //Randomness manipulation
        int min = (int)(PlantMinSize - currentTax * RNGManipulation);
        int max = (int)(PlantMaxSize + 1 - currentTax * RNGManipulation);
        int plantSize = UnityEngine.Random.Range(min, max);
        plantSize = Math.Clamp(plantSize, PlantMinSize, PlantMaxSize);
        currentTax -= AveragePlantSize - plantSize;
        
        while(plantPositions.Count() >= plantSize)
        {
            Vector2Int seed = plantPositions.ElementAt(UnityEngine.Random.Range(0, plantPositions.Count));
            Vector2Int newTile = GetRandAdjacent(seed, plantPositions);
            plantPositions.Add(newTile);
        }

        return (Vector2Int position) => LocalToGlobal(plantPositions, position);
    }

    public PlantGenerator GetPlantGenerator() => CreatePlantShape();

    private HashSet<Vector2Int> LocalToGlobal(HashSet<Vector2Int> localPositions, Vector2Int globalPosition)
    {
        HashSet<Vector2Int> globalPositions = new();

        foreach (Vector2Int position in localPositions)
            globalPositions.Add(position + globalPosition);

        return globalPositions;
    }

    private Vector2Int GetRandAdjacent(Vector2Int cell, HashSet<Vector2Int> blackList = null)
    {
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            new(cell.x - 1, cell.y), // left
            new(cell.x + 1, cell.y), // right
            new(cell.x, cell.y - 1), // up
            new(cell.x, cell.y + 1)  // down
        };

        if (blackList != null)
            adjacentPositions = adjacentPositions.Where(pos => !blackList.Contains(pos)).ToArray();

        if (adjacentPositions.Length == 0)
            return cell; //If all positions are blacklisted, then return origin position

        return adjacentPositions[UnityEngine.Random.Range(0, adjacentPositions.Length)];
    }
}

public delegate HashSet<Vector2Int> PlantGenerator(Vector2Int rootPosition);