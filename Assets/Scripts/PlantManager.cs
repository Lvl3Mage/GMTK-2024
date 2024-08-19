#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lvl3Mage.EditorEnhancements.Runtime;
using UnityEngine;

public class PlantManager : MonoBehaviour
{ 
    List<Plant> plants = new();
    [SerializeField] Plant plantPrefab;
    [SerializeField] float plantShakeTime = 1f;
    [SerializeField] float plantAnimationTime = 1f;

    /// <summary>
    /// Create a new Plant with all its functions.
    /// </summary>
    /// <param name="positions">the coordinates where it will be created</param>
    /// <param name="rootPosition">the coordinates of the first Plant</param>
    public void SpawnPlant(HashSet<Vector2Int> positions, Vector2Int rootPosition)
    {
        Vector3 position = (Vector2)rootPosition;
        position.z = transform.position.z;
        Plant newPlant = Instantiate(plantPrefab, position, Quaternion.identity);
        newPlant.Create(positions, rootPosition);
        plants.Add(newPlant);
        Debug.Log($"Plant created at root position {rootPosition}, plant count {plants.Count}");
        newPlant.OnDestroyed += RemovePlant;
    }

    void RemovePlant(Plant plant)
    {
        plants.Remove(plant);
        Debug.Log($"Plant removed from root position {plant.transform.position}");
    }


    public IEnumerator UpdatePlants()
    {
        for (int index = plants.Count-1; index >= 0; index--){
            Plant plant = plants[index];
            plant.Grow();
        }

        HashSet<Vector2Int> plantAdditions = WorldGrid.instance.GetPlantAdditions();
        HashSet<Vector2Int> plantRemovals = WorldGrid.instance.GetPlantRemovals();
        WorldGrid.instance.ClearPlantChanges();
        
        Dictionary<Plant, HashSet<Vector2Int>> plantGroups = new();
        foreach (Vector2Int addition in plantAdditions){
            Plant? plant = WorldGrid.instance.GetPlantAt(addition);
            if (plant == null){
                Debug.LogWarning("Plant not found at addition position");
                continue;
            }
            if (!plantGroups.ContainsKey(plant)){
                plantGroups[plant] = new HashSet<Vector2Int>();
            }
            plantGroups[plant].Add(addition);
        }
        PlantRenderer.FillGroup[] fillGroups = new PlantRenderer.FillGroup[plantGroups.Count];
        int i = 0;
        foreach (KeyValuePair<Plant, HashSet<Vector2Int>> plantGroup in plantGroups){
            fillGroups[i] = new PlantRenderer.FillGroup(plantGroup.Value, plantGroup.Key.PlantColor);
            i++;
        }
        PlantRenderer.instance.RemoveFilledCells(plantRemovals);
        PlantRenderer.instance.AddFilledCells(fillGroups);
        
        PlantRenderer.instance.InitiateShake();
        yield return new WaitForSeconds(plantShakeTime);
        
        PlantRenderer.instance.InitiateSpriteChange();
        yield return new WaitForSeconds(plantAnimationTime);
        
    }

    public int GetPlantCount()
    {
        return plants.Count;
    }
}

