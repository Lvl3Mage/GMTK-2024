using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    private List<Plant> plants = new List<Plant>();
    [SerializeField] Plant plantPrefab;

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
        foreach (var plant in plants)
        {
           // Grow Method
        }
    }

    private Plant InstantiatePlantAt(Vector2Int position)
    {
        GameObject plantObject = new GameObject("Plant");
        Plant plant = Instantiate(plantPrefab);

        plant.transform.position = new Vector2(position.x, position.y);
        return plant;
    }
}
