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
    public void SpawnPlant(Vector2Int[] positions, Vector2Int rootPosition)
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


    public IEnumerator UpdatePlants()
    {
        foreach (Plant plant in plants)
        {
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
        WorldRenderer.FillGroup[] fillGroups = new WorldRenderer.FillGroup[plantGroups.Count];
        int i = 0;
        foreach (KeyValuePair<Plant, HashSet<Vector2Int>> plantGroup in plantGroups){
            fillGroups[i] = new WorldRenderer.FillGroup(plantGroup.Value, plantGroup.Key.PlantColor);
            i++;
        }
        WorldRenderer.instance.RemoveFilledCells(plantRemovals);
        WorldRenderer.instance.AddFilledCells(fillGroups);
        
        WorldRenderer.instance.InitiateShake();
        yield return new WaitForSeconds(plantShakeTime);
        WorldRenderer.instance.InitiateSpriteChange();
        yield return new WaitForSeconds(plantAnimationTime);
        
    }
    
}

