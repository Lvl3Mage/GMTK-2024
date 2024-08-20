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
using Random = UnityEngine.Random;

class PlantSelector : MonoBehaviour
{
    [SerializeField] int belowSizeOffset = 5;
    [SerializeField] int aboveSizeOffset = 1;
    /// <summary>
    /// The approximate amount of cells the player will have empty before going next stage
    /// </summary>
    [SerializeField] int sparedCellsMargin = 5;
    [SerializeField] int minPlantSize = 3;
    [Range(0, 1)][SerializeField] float RNGManipulation = 0.5f;
    int targetPlantSize = 6;
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
        //Calculate average plant size on current stage
        int freeCells = WorldGrid.instance.GetFreeCellAmount();
        targetPlantSize = (freeCells - sparedCellsMargin) / (GameManager.instance.GetCurrentGoal() - PlantManager.instance.GetPlantCount());
        //Randomness manipulation
        int min = (int)(targetPlantSize - belowSizeOffset - currentTax * RNGManipulation);
        int max = (int)(targetPlantSize + aboveSizeOffset + 1 - currentTax * RNGManipulation);

        int plantSize = UnityEngine.Random.Range(min, max);
        plantSize = Math.Clamp(plantSize, minPlantSize, Math.Max(minPlantSize, targetPlantSize + aboveSizeOffset));
        currentTax -= targetPlantSize - plantSize;
        
        HashSet<Vector2Int> plantPositions = GenerateShapeWithSize(plantSize);
        // while(plantPositions.Count < plantSize)
        // {
        //     Vector2Int seed = plantPositions.ElementAt(UnityEngine.Random.Range(0, plantPositions.Count));
        //     Vector2Int newTile = GetRandAdjacent(seed, plantPositions);
        //     plantPositions.Add(newTile);
        // }

        return position => LocalToGlobal(plantPositions, position);
    }

    public PlantGenerator GetPlantGenerator() => CreatePlantShape();

    private HashSet<Vector2Int> LocalToGlobal(HashSet<Vector2Int> localPositions, Vector2Int globalPosition)
    {
        HashSet<Vector2Int> globalPositions = new();

        foreach (Vector2Int position in localPositions)
            globalPositions.Add(position + globalPosition);

        return globalPositions;
    }

    HashSet<Vector2Int> GenerateShapeWithSize(int size)
    {
        const int sampleCount = 6;
        HashSet<Vector2Int> possibleStartingPositions = GetOpenCells();
        Debug.Log("Possible starting positions: " + possibleStartingPositions.Count);
        HashSet<Vector2Int> startingPositions = new();
        for (int i = 0; i < sampleCount; i++){
            if (possibleStartingPositions.Count == 0){
                break;
            }
            startingPositions.Add(possibleStartingPositions.ElementAt(UnityEngine.Random.Range(0, possibleStartingPositions.Count)));
        }

        HashSet<Vector2Int> bestShape = new(){Vector2Int.zero};
        foreach (Vector2Int startingPosition in startingPositions){
            Debug.Log("Trying to generate shape from " + startingPosition);
            HashSet<Vector2Int> shape = GenerateValidShapeFromOffsets(startingPosition, size);
            if (shape.Count > bestShape.Count){
                bestShape = shape;
            }
            Debug.Log("Shape size: " + shape.Count);
        }

        return bestShape;




    }

    HashSet<Vector2Int> GenerateValidShapeFromOffsets(Vector2Int worldPosition, int targetSize)
    {
        Debug.Log("Generating shape from targetSize " + targetSize);
        HashSet<Vector2Int> offsets = new();
        List<Vector2Int> offsetsToCheck = new();
        offsetsToCheck.Add(Vector2Int.zero);
        while (offsetsToCheck.Count > 0 && offsets.Count < targetSize){
            int index = Random.Range(0, offsetsToCheck.Count);
            Debug.Log("Index: " + index);
            Vector2Int offset = offsetsToCheck[index];
            offsetsToCheck.RemoveAt(index);
            offsetsToCheck.Remove(offset);
            if (offsets.Contains(offset)){
                continue;
            }
            Vector2Int worldPos = offset + worldPosition;
            // Debug.Log("Checking " + worldPos);
            if (WorldGrid.instance.CellTargetable(worldPos) && !WorldGrid.instance.GetGrowthAt(worldPos)){
                offsets.Add(offset);
                // Debug.Log("Added " + offset);
                Vector2Int[] neighbours = CellUtils.GetTrueCellNeighbours(offset);
                foreach (Vector2Int neighbour in neighbours){
                    offsetsToCheck.Add(neighbour);
                }
            }
        }

        return offsets;
    }
    
    
    HashSet<Vector2Int> GetOpenCells()
    {
        Vector2Int gridSize = WorldGrid.instance.GridSize;
        HashSet<Vector2Int> openCells = new();
        for (int i = 0; i < gridSize.x; i++){
            for (int j = 0; j < gridSize.y; j++){
                Vector2Int cell = new(i, j);
                cell -= gridSize / 2;
                Debug.Log("Checking cell " + cell);
                if (WorldGrid.instance.CellTargetable(cell) && !WorldGrid.instance.GetGrowthAt(cell)){
                    Debug.Log("Cell is open");
                    openCells.Add(cell);
                }
            }
        }

        return openCells;
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