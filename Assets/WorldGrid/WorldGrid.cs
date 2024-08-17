using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable


public class WorldGrid : MonoBehaviour
{
    private Dictionary<Vector2Int, WorldGrid.Plant> plantLookUp = new Dictionary<Vector2Int, WorldGrid.Plant>();
    private Dictionary<Vector2Int, int> growthLookUp = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, MapCellType> mapLookUp = new Dictionary<Vector2Int, MapCellType>();

    // Como sabemos que solo habra un solo WorldGrid en una escena, hacemos esto para no tener que buscarlo por "tag"
    public static WorldGrid instance 
    { 
        get;
        private set;
    }

    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("another instance of WorldGrid exists"); // si existiese otro WorldGrid se autodestruiria
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void RegisterPlant(Vector2Int position, Plant plant)
    {
        if(!plantLookUp.ContainsKey(position))
        {
            plantLookUp.Add(position, plant);
        }
    }

    public void RemovePlant(Vector2Int position)
    {
        if(plantLookUp.ContainsKey(position))
        {
            plantLookUp.Remove(position);
        }
    }

    public Plant? GetPlantAt(Vector2Int position)
    {
        if(plantLookUp.ContainsKey(position))
        { 
            return plantLookUp[position]; 
        }
        else { return null; }
    }

    public bool GetGrowthAt(Vector2Int position)
    {
        if(plantLookUp.ContainsKey(position))
            return false;
        return true;
        
    }

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

    public void RemoveGrowthPositions(Vector2Int[] positions)
    {
        foreach (var position in positions)
        {
            if (growthLookUp.ContainsKey(position))
            {
                growthLookUp.Remove(position);
            }
        }
    }

    public MapCellType GetMapTypeAt(Vector2Int position)
    {
        mapLookUp.TryGetValue(position, out MapCellType mapCellType);
        return mapCellType;
    }


    public class Plant
    {
        public int id;
    }
}


public enum MapCellType
{
    Water,
    Land
}