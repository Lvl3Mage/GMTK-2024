#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lvl3Mage.EditorEnhancements.Runtime;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public event Action OnPlantDestroyed;
    public event Action OnPlantCreated;
    public event Action OnPlantUpdated;
    public static PlantManager instance { get; private set; }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another instance of PlantManager exists!");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    HashSet<Plant> managedPlants = new();
    [SerializeField] Plant plantPrefab;
    [SerializeField] float plantShakeTime = 1f;
    [SerializeField] float plantAnimationTime = 1f;

    /// <summary>
    /// Create a new Plant with all its functions.
    /// </summary>
    /// <param name="positions">the coordinates where it will be created</param>
    /// <param name="rootPosition">the coordinates of the first Plant</param>
    public IEnumerator SpawnPlant(HashSet<Vector2Int> positions, Vector2Int rootPosition)
    {
        Vector3 position = (Vector2)rootPosition;
        position.z = transform.position.z;
        Plant newPlant = Instantiate(plantPrefab, position, Quaternion.identity);
        yield return newPlant.Create(positions, rootPosition);
        managedPlants.Add(newPlant);
        yield return UpdatePlant(newPlant);
    }
    void DestroyDisconnectedPlants()
    {
        HashSet<Plant> connectedPlants = PlantDFS(WorldGrid.instance.GetWaterNeighbouringPlants());
        HashSet<Plant> disconnectedPlants = new(managedPlants);
        disconnectedPlants.ExceptWith(connectedPlants);
        foreach (Plant plant in disconnectedPlants)
        {
            managedPlants.Remove(plant);
            plant.DestroyPlant();
        }
    }

    static HashSet<Plant> PlantDFS(HashSet<Plant> sourcePlants)
    {
        
        Debug.Log($"InitialPlants: {sourcePlants.Count}");
        HashSet<Plant> connectedPlants = new(sourcePlants);
        Queue<Plant> plantQueue = new Queue<Plant>(connectedPlants);
        
        while(plantQueue.Count > 0)
        {
            Plant plant = plantQueue.Dequeue();
            HashSet<Plant> neighbours = plant.GetNeighbours();
            neighbours.ExceptWith(connectedPlants);
            
            foreach(Plant neighbour in neighbours){
                connectedPlants.Add(neighbour);
                plantQueue.Enqueue(neighbour);
            }
        }

        return connectedPlants;
        
    }
    public void DestroyPlant(Plant plant)
    {
        Debug.Log("Destroying plant");
        managedPlants.Remove(plant);
        plant.DestroyPlant();
        DestroyDisconnectedPlants();
        Debug.Log($"Plant cleanup plants remaining {managedPlants.Count}");
        
    }

    IEnumerator UpdatePlant(Plant plant)
    {
        plant.Grow();
        HashSet<Vector2Int> plantAdditions = WorldGrid.instance.GetPlantAdditions(); 
        HashSet<Vector2Int> plantRemovals = WorldGrid.instance.GetPlantRemovals();
        WorldGrid.instance.ClearPlantChanges();
        
        Dictionary<Plant, HashSet<Vector2Int>> plantGroups = new();
        foreach (Vector2Int addition in plantAdditions){
            Plant? addedPlant = WorldGrid.instance.GetPlantAt(addition);
            if (addedPlant == null){
                Debug.LogWarning("Plant not found at addition position");
                continue;
            }

            if (!plantGroups.ContainsKey(addedPlant)){
                plantGroups[addedPlant] = new HashSet<Vector2Int>();
            }

            plantGroups[addedPlant].Add(addition);
        }
        int i = 0;
        PlantRenderer.FillGroup[] fillGroups = new PlantRenderer.FillGroup[plantGroups.Count];
        foreach (KeyValuePair<Plant, HashSet<Vector2Int>> plantGroup in plantGroups){
            fillGroups[i] = new PlantRenderer.FillGroup(plantGroup.Value, plantGroup.Key.PlantColor);
            i++;
        }
        PlantRenderer.instance.RemoveFilledCells(plantRemovals);
        PlantRenderer.instance.AddFilledCells(fillGroups);
        

        if (PlantRenderer.instance.RequiresShake()){
            PlantRenderer.instance.InitiateShake();
            yield return new WaitForSeconds(plantShakeTime);
            SoundController.instance.PlayMusicKeys();
            
        }

        if (PlantRenderer.instance.RequiresSpriteChange()){
            PlantRenderer.instance.InitiateSpriteChange();
            SoundController.instance.PlayEffectSound();
            yield return new WaitForSeconds(plantAnimationTime);
        }
    }
    public IEnumerator UpdatePlants()
    {
        HashSet<Plant> plantsToUpdate = new(managedPlants);
        while(plantsToUpdate.Count > 0)
        {
            Plant plant = plantsToUpdate.First();
            if (managedPlants.Contains(plant)){
                yield return UpdatePlant(plant);
            }
            plantsToUpdate.Remove(plant);
            
        }
    }

    public int GetPlantCount()
    {
        return managedPlants.Count;
    }
}

