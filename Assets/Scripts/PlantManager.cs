using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{ 
    List<Plant> plants = new();
    [SerializeField] Plant plantPrefab;


    /// <summary>
    /// Create a new Plant with all its functions.
    /// </summary>
    /// <param name="positions">the coordinates where it will be created</param>
    /// <param name="rootPosition">the coordinates of the first Plant</param>
    void CreatePlant(Vector2Int[] positions, Vector2Int rootPosition)
    {
        Plant newPlant = Instantiate(plantPrefab, (Vector2)rootPosition, Quaternion.identity);
        newPlant.Create(positions, rootPosition);
        plants.Add(newPlant);
        newPlant.OnDestroyed += RemovePlant;
    }
    
    
    

    void RemovePlant(Plant plant)
    {
        plants.Remove(plant);
        Debug.Log($"Plant removed from root position {plant.transform.position}");
    }

    public void UpdatePlants()
    {
        foreach (Plant plant in plants)
        {
            plant.Grow();
        }
    }
    
}
