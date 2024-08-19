using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lvl3Mage.CameraManagement2D;
public class GameManager : MonoBehaviour
{
    [SerializeField] CameraModuleManager cameraModuleManager;
    [SerializeField] CameraPanModule cameraPanModule;
    [SerializeField] CameraGridController cameraGridController;

    [SerializeField] PlantManager plantManager;
    [SerializeField] PlantCreator plantCreator;
    [SerializeField] PlantSelector plantSelector;
    
    [SerializeField] Vector2Int gridSize;
    [SerializeField] Vector2Int gridExpandAmount;

    int level = 0;
    [SerializeField] float minZoom;
    void Start()
    {
        Bounds bounds = WorldGrid.instance.InitializeBounds(gridSize);
        cameraPanModule.SetClamp(cameraGridController.GetCameraClamp(bounds));
        StartCoroutine(RunGame());
    }

    bool LevelComplete(int level)
    {
        return plantManager.GetPlantCount() > level;
    }

    IEnumerator RunGame()
    {
        while (true){
            yield return RunLevel();
            level++;
            yield return ExpandGrid(gridExpandAmount);
        }
    }
    
    IEnumerator RunLevel()
    {
        while (!LevelComplete(level)){
            yield return plantSelector.SelectPlant();
            
            PlantGenerator plantGenerator = plantSelector.GetPlantGenerator();
            yield return plantCreator.CreatePlant(plantGenerator);
            
            
            Vector2Int[] plantPositions = plantCreator.GetPlantPositions();
            Vector2Int rootPosition = plantCreator.GetRootPosition();
            plantManager.SpawnPlant(plantPositions, rootPosition);
            yield return plantManager.UpdatePlants();
            
        }
        
    }
    IEnumerator ExpandGrid(Vector2Int amount)
    {
        gridSize += amount;
        Bounds newBounds = WorldGrid.instance.ExpandGridBounds(amount);
        cameraPanModule.SetClamp(cameraGridController.GetCameraClamp(newBounds));
        cameraModuleManager.SwitchToController(1);
        yield return cameraGridController.ExpandTo(newBounds);
        //Todo camera controller should expand to grid bounds
        cameraPanModule.AdaptToState(cameraGridController.GetCameraState());
        cameraModuleManager.SwitchToController(0);
    }
    public static GameManager instance { get; private set; }
    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another instance of GameManager exists!");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}