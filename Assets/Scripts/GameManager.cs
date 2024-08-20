using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lvl3Mage.CameraManagement2D;
using TMPro;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public event Action OnGrowStart;
    public event Action OnGrowEnd;
    public event Action OnLevelComplete;
    public event Action OnGridExpanded;
    public event Action OnLevelStart;
    public event Action OnNewPlant;
    [SerializeField] CameraModuleManager cameraModuleManager;
    [SerializeField] CameraPanModule cameraPanModule;
    [SerializeField] CameraGridController cameraGridController;

    [SerializeField] PlantManager plantManager;
    [SerializeField] PlantCreator plantCreator;
    [SerializeField] PlantSelector plantSelector;
    
    [SerializeField] Vector2Int gridSize;
    [SerializeField] Vector2Int gridExpandAmount;

    [SerializeField] MapRenderer map;

    [SerializeField] TextWriter plantRequirementDisplay;
    [SerializeField] int[] stagePlantRequirements;
    [SerializeField] float requirementsPerAddedArea;
    int stage = 0;
    void Start()
    {
        Bounds bounds = WorldGrid.instance.InitializeBounds(gridSize);
        cameraPanModule.SetClamp(cameraGridController.GetCameraClamp(bounds));
        StartCoroutine(RunGame());

        map.FillBounds();
        StartCoroutine(map.renderWorldGrid());
    }

    public int GetPlantRequirement(int currentStage)
    {
        if(currentStage < stagePlantRequirements.Length){
            return stagePlantRequirements[currentStage];
        }
        int lastDefinedRequirement = stagePlantRequirements[stagePlantRequirements.Length - 1];
        Vector2Int lastDefinedStageSize = CalculateGridSize(stagePlantRequirements.Length - 1);
        Vector2Int currentStageSize = CalculateGridSize(currentStage);
        float addedArea = (currentStageSize.x * currentStageSize.y) - (lastDefinedStageSize.x * lastDefinedStageSize.y);
        return lastDefinedRequirement + (int)(addedArea * requirementsPerAddedArea);
    }
    Vector2Int CalculateGridSize(int level)
    {
        return gridSize + level * gridExpandAmount;
    }
    void Update()
    {
        plantRequirementDisplay.Set(GetPlantRequirement(stage));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraModuleManager.UseUserInput(false);
        }
        else{
            cameraModuleManager.UseUserInput(true);
            
        }
    }

    bool IsStageComplete(int currentStage)
    {
        return plantManager.GetPlantCount() >= GetPlantRequirement(currentStage);
    }

    public int GetCurrentGoal() => GetPlantRequirement(stage);

    IEnumerator RunGame()
    {
        while (true){
            yield return RunLevel();
            stage++;
            OnLevelComplete?.Invoke();
            yield return ExpandGrid();
            OnGridExpanded?.Invoke();
        }
    }
    
    IEnumerator RunLevel()
    {
        OnLevelStart?.Invoke();
        while (!IsStageComplete(stage)){
            OnNewPlant?.Invoke();
            yield return plantSelector.SelectPlant();
            
            PlantGenerator plantGenerator = plantSelector.GetPlantGenerator();
            yield return plantCreator.CreatePlant(plantGenerator);
            
            
            HashSet<Vector2Int> plantPositions = plantCreator.GetPlantPositions();
            Vector2Int rootPosition = plantCreator.GetRootPosition();
            OnGrowStart?.Invoke();
            yield return plantManager.SpawnPlant(plantPositions, rootPosition);
            yield return plantManager.UpdatePlants();   
            OnGrowEnd?.Invoke();
        }
        
    }
    IEnumerator ExpandGrid()
    {
        SoundController.instance.PlayGong();

        SoundController.instance.PlayRope();
        Vector2Int newGridSize = CalculateGridSize(stage);
        Vector2Int expandAmount = newGridSize - gridSize;
        gridSize = newGridSize;
        Bounds newBounds = WorldGrid.instance.ExpandGridBounds(expandAmount);
        CameraState finalState = cameraGridController.GetMaxGridState(newBounds);
        cameraModuleManager.SwitchToController(1);
        cameraPanModule.SetClamp(cameraGridController.GetCameraClamp(newBounds));
        cameraPanModule.AdaptToState(finalState);
        yield return cameraGridController.ExpandTo(newBounds);
        //Todo camera controller should expand to grid bounds
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
    
    bool uiVisible = false;
    public void ToggleUI(bool val)
    {
        uiVisible = val;
    }
    public bool IsUIVisible() => uiVisible;
}