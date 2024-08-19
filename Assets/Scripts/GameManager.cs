using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lvl3Mage.CameraManagement2D;
public class GameManager : MonoBehaviour
{
    [SerializeField] CameraModuleManager cameraModuleManager;
    [SerializeField] CameraPanModule cameraPanModule;

    [SerializeField] PlantManager plantManager;
    [SerializeField] PlantCreator plantCreator;
    [SerializeField] PlantSelector plantSelector;
    
    [SerializeField] Vector2Int gridSize;
    [SerializeField] Vector2Int gridExpandAmount;

    int level = 0;
    [SerializeField] float minZoom;
    [SerializeField] float cameraBoundsPadding;
    void Start()
    {
        Bounds bounds = WorldGrid.instance.InitializeBounds(gridSize);
        cameraPanModule.SetClamp(GetCameraClamp(bounds));
        StartCoroutine(RunGame());
    }

    bool LevelComplete(int level)
    {
        int gridArea = gridSize.x * gridSize.y;
        int plantCount = WorldGrid.instance.GetPlantCount();
        return plantCount/15f >= gridArea;
    }
    IEnumerator RunGame()
    {
        while (true){
            yield return RunLevel();
            
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
        CameraStateClamp cameraStateClamp = GetCameraClamp(newBounds);
        cameraPanModule.SetClamp(cameraStateClamp);
        cameraModuleManager.SwitchToController(1);
        
        //Todo camera controller should expand to grid bounds
        
        cameraPanModule.AdaptToCamera();
        cameraModuleManager.SwitchToController(0);
        
        
        yield return null;
    }

    CameraStateClamp GetCameraClamp(Bounds bounds)
    {
        bounds = new Bounds(bounds.center, bounds.size);
        Debug.Log($"Camera bounds: {bounds}");
        bounds.Expand(cameraBoundsPadding*2);
        Debug.Log($"Camera bounds: {bounds}");
        float maxZoom = Mathf.Max(bounds.size.x * SceneCamera.GetCamera().aspect, bounds.size.y);
        
        
        return new CameraStateClamp(CameraStateClamp.ClampMode.ClampPosition, bounds,
            new Vector2(minZoom, maxZoom));
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