using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable


public class WorldGrid : MonoBehaviour
{
    Bounds gridBounds;
    public Bounds InitializeBounds(Vector2Int gridSize)
    {
        gridBounds = new Bounds();
        gridBounds.Expand((Vector2)gridSize*2);
        return gridBounds;
    }
    public Bounds GridBounds => gridBounds;
    public Vector2Int GridSize => new Vector2Int((int)gridBounds.size.x + 1, (int)gridBounds.size.y + 1);
    public void SetGridBounds(Bounds bounds)
    {
        gridBounds = bounds;
    }
    public Bounds ExpandGridBounds(Vector2Int amount)
    {
        gridBounds.Expand(new Vector3(amount.x*2, amount.y*2, 0));
        return gridBounds;
    }
    
    /// <summary>
    /// Check whether the cell supports plant growth
    /// </summary>
    /// <param name="position">
    /// The cell to check
    /// </param>
    /// <returns>
    /// True if the cell supports plant growth. Otherwise false
    /// </returns>
    public bool CellTargetable(Vector2Int position)
    {
        MapCellType cellType = GetMapTypeAt(position);
        if (cellType == MapCellType.Water)
        {
            return false;
        }
        return gridBounds.Contains((Vector2)position);
    }
    readonly Dictionary<Vector2Int, Plant> plantLookUp = new();
    readonly Dictionary<Vector2Int, int> growthLookUp = new();
    HashSet<Vector2Int> waterPositions = new();

    public void AddWaterPosition(Vector2Int pos)
    {
        waterPositions.Add(pos);
    }

    [SerializeField] Vector2Int[] initialWaterPosition;

    public int GetFreeCellAmount()
    {
        int totalCells = GridSize.x * GridSize.y;
        int obstructedCells = growthLookUp.Count + waterPositions.Count;
        int freeCells = totalCells - obstructedCells;

        return freeCells;
    }

    void OnDrawGizmos()
    {
        foreach (var plant in plantLookUp)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere((Vector2)plant.Key, 0.3f);
        }
        
        foreach(var addedCell in addedCells)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere((Vector2)addedCell, 0.1f);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gridBounds.center, gridBounds.size);
    }

    /// <summary>
    /// Tracks the cells that have been added from the grid since the ClearPlantChanges call
    /// </summary>
    readonly HashSet<Vector2Int> addedCells = new();
    
    /// <summary>
    /// Tracks the cells that have been removed from the grid since the ClearPlantChanges call
    /// </summary>
    readonly HashSet<Vector2Int> removedCells = new();
    
    /// <summary>
    /// WorldGrid Singleton instance
    /// </summary>
    public static WorldGrid instance { get; private set; } 

    /// <summary>
    /// Initializes the WorldGrid Singleton
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another instance of WorldGrid exists! Destroying this one ."); // si existiese otro WorldGrid se autodestruiria
            Destroy(gameObject);
            return;
        }
        instance = this;

        waterPositions.UnionWith(initialWaterPosition);
    }

    public void RegisterPlant(Vector2Int position, Plant plant)
    {
        if(plantLookUp.ContainsKey(position)){
            if(plantLookUp[position] == plant)
            {
                Debug.LogWarning("Trying to register the same plant twice");
                return;
            }
        }

        plantLookUp.Add(position, plant);
        addedCells.Add(position);
    }

    public HashSet<Plant> GetWaterNeighbouringPlants()
    {
        HashSet<Plant> waterPlants = new HashSet<Plant>();
        
        foreach(Vector2Int waterPosition in waterPositions){
            Vector2Int[] neighbours = CellUtils.GetTrueCellNeighbours(waterPosition);
            foreach (Vector2Int neighbour in neighbours){
                Plant? plant = GetPlantAt(neighbour);
                if(plant != null)
                {
                    waterPlants.Add(plant);
                }
            }
        }

        return waterPlants;
    }

    public void RemovePlantAt(Vector2Int position)
    {
        if(plantLookUp.ContainsKey(position))
        {
            plantLookUp.Remove(position);
            removedCells.Add(position);
        }
        else
        {
            Debug.LogWarning("Trying to erase a non-existent plant");
        }
    }

    public Plant? GetPlantAt(Vector2Int position)
    {
        if(plantLookUp.ContainsKey(position))
        { 
            return plantLookUp[position]; 
        }

        return null;
    }

    public HashSet<Vector2Int> GetPlantAdditions()
    {
        return new HashSet<Vector2Int>(addedCells);
    }
    public HashSet<Vector2Int> GetPlantRemovals()
    {
        return new HashSet<Vector2Int>(removedCells);
    }
    public void ClearPlantChanges()
    {
        addedCells.Clear();
        removedCells.Clear();
    }

    /// <summary>
    /// Check the growth at a specific cell
    /// </summary>
    /// <param name="position">
    /// The cell to check
    /// </param>
    /// <returns>
    /// Returns whether any plants have targeted a cell for growth
    /// </returns>
    public bool GetGrowthAt(Vector2Int position)
    {
        return !plantLookUp.ContainsKey(position);
    }

    /// <summary>
    /// Add growth targeted positions
    /// </summary>
    /// <param name="positions">
    /// The positions to add
    /// </param>

    public void AddGrowthPositions(Vector2Int[] positions)
    {   
        
        foreach (var position in positions)
        {
            if (!growthLookUp.ContainsKey(position))
            {
                growthLookUp.Add(position, 0);
            }
            growthLookUp[position]++;
        }
    }
    /// <summary>
    /// Remove growth targeted positions
    /// </summary>
    /// <param name="positions">
    /// The positions to remove
    /// </param>
    public void RemoveGrowthPositions(HashSet<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            if (growthLookUp.ContainsKey(position))
            {
                growthLookUp.Remove(position);
            }
        }
    }

    
    //Map Lookup

    public MapCellType GetMapTypeAt(Vector2Int position)
    {
        if (waterPositions.Contains(position))
        {
            return MapCellType.Water;
        }
        return MapCellType.Land;
    }

    public HashSet<Vector2Int> GetWaterCells() => new(waterPositions);

    public int GetPlantCellCount()
    {
        return plantLookUp.Count;
    }
}