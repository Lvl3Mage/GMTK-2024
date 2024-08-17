using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    
    private List<Plant> plants = new List<Plant>();
    [SerializeField] Plant plantPrefab;


    /// <summary>
    /// Create a new Plant with all its functions.
    /// </summary>
    /// <param name="positions">the coordinates where it will be created</param>
    /// <param name="rootPosition">the coordinates of the first Plant</param>
    public void CreatePlant(Vector2Int[] positions, Vector2Int rootPosition)
    {
        Plant newPlant = InstantiatePlantAt(rootPosition);
        newPlant.Create(positions, rootPosition);

        //foreach (var position in positions)
        //{
        //    newPlant.AddPlantPosition(position);
        //}
        plants.Add(newPlant);
        newPlant.OnDestroyed += RemovePlant;
    }

    private void RemovePlant(Plant plant)
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

    private Plant InstantiatePlantAt(Vector2Int position)
    {
        Plant plant = Instantiate(plantPrefab);

        plant.transform.position = new Vector2(position.x, position.y);
        return plant;
    }
static float easeInBack(float x) {
    const float c1 = 1.70158f;
    const float c3 = c1 + 1;

    return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
}
    
}
