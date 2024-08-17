using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable


public class WorldGrid : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, Plant> plantLookUp = new Dictionary<Vector2Int, Plant>();
    private readonly Dictionary<Vector2Int, int> growthLookUp = new Dictionary<Vector2Int, int>();
    private readonly Dictionary<Vector2Int, MapCellType> mapLookup = new Dictionary<Vector2Int, MapCellType>();

    // Como sabemos que solo habra un solo WorldGrid en una escena, hacemos esto para no tener que buscarlo por "tag"
    public static WorldGrid instance { get; private set; }

    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another instance of WorldGrid exists!"); // si existiese otro WorldGrid se autodestruiria
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // ------ Metodos de plantLookUp ------

    public void RegisterPlant(Vector2Int position, Plant plant)
    {
        if (plantLookUp.ContainsKey(position)){
            Debug.LogWarning($"existing plant in {position}");
        }

        plantLookUp.Add(position, plant);
    }

    public void RemovePlantAt(Vector2Int position)
    {
        if(plantLookUp.ContainsKey(position))
        {
            plantLookUp.Remove(position);
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


    public bool GetGrowthAt(Vector2Int position)
    {
        return !plantLookUp.ContainsKey(position);
    }

    // ------ Metodos de growthLookUp ------

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

    // ------ Metodos de mapLookUp ------

    public MapCellType GetMapTypeAt(Vector2Int position)
    {
        if(mapLookup.ContainsKey(position))
        {
            return mapLookup[position];
        }
        return MapCellType.Land;
    }

}


public enum MapCellType
{
    Land,
    Water
}