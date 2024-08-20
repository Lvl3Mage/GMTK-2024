using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lvl3Mage.CameraManagement2D;
using TMPro;

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

    [SerializeField] MapRenderer map;
    [SerializeField] TextMeshProUGUI text;

    int level = 5;
    int scaleLevel = 10;
    [SerializeField] float minZoom;
    void Start()
    {
        Bounds bounds = WorldGrid.instance.InitializeBounds(gridSize);
        cameraPanModule.SetClamp(cameraGridController.GetCameraClamp(bounds));
        StartCoroutine(RunGame());

        map.FillBounds();
        StartCoroutine(map.renderWorldGrid());
    }

    private void Update()
    {
        text.text = (level - plantManager.GetPlantCount() + 1).ToString();
    }

    bool LevelComplete(int currentlevel)
    {
        if(plantManager.GetPlantCount() > currentlevel)
        {
            level = scaleLevel;
            scaleLevel *= 2;
            return true;
        }

        return false;
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
            
            
            HashSet<Vector2Int> plantPositions = plantCreator.GetPlantPositions();
            Vector2Int rootPosition = plantCreator.GetRootPosition();
            plantManager.SpawnPlant(plantPositions, rootPosition);
            yield return plantManager.UpdatePlants();   
        }
        
    }
    IEnumerator ExpandGrid(Vector2Int amount)
    {
        SoundController.instance.PlayGong();

        SoundController.instance.PlayRope();
        gridSize += amount;


        Bounds newBounds = WorldGrid.instance.ExpandGridBounds(amount);
        cameraPanModule.SetClamp(cameraGridController.GetCameraClamp(newBounds));
        cameraModuleManager.SwitchToController(1);
        yield return cameraGridController.ExpandTo(newBounds);
        //Todo camera controller should expand to grid bounds
        cameraPanModule.AdaptToState(cameraGridController.GetCameraState());
        cameraModuleManager.SwitchToController(0);
        
        
        map.FillBounds();
        StartCoroutine(map.renderWorldGrid());

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