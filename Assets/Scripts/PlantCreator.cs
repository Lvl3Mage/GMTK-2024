using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lvl3Mage.CameraManagement2D;
using UnityEngine;

public class PlantCreator : MonoBehaviour
{
	public event Action OnPlantSpawned;
	public event Action OnPlantCreated;
	
	
    [SerializeField] SpriteRenderer previewSpritePrefab;
    [SerializeField] Color previewColor;
    [SerializeField] Color previewErrorColor;
    [SerializeField] Color previewOccupiedColor;
    [SerializeField] Color previewHiddenColor;
    [SerializeField] Color previewHoverColor;
    PlantGenerator plantGenerator = (position) => {
	    HashSet<Vector2Int> points = new HashSet<Vector2Int>();
	    points.Add(position);
	    return points;
    };

    int rotationAmount = 0;

	Vector2Int targetSpawnPosition;
	Vector2Int targetRootPosition;

	bool IsValidRoot(Vector2Int rootPosition)
	{
		if (WorldGrid.instance.GetPlantAt(rootPosition) != null){
			return false;
		}
		Vector2Int[] neighbours = CellUtils.GetTrueCellNeighbours(rootPosition);
		foreach (Vector2Int neighbour in neighbours){
			if (WorldGrid.instance.GetPlantAt(neighbour) != null){
				return true;
			}
			if(WorldGrid.instance.GetMapTypeAt(neighbour) == MapCellType.Water){
				return true;
			}
		}

		return false;
	}
    public IEnumerator CreatePlant(PlantGenerator generator)
    {
	    rotationAmount = 0;
	    plantGenerator = generator;
	    while(!Input.GetMouseButtonDown(0) || !CurrentSpawnValid(GetCurrentPositions()))
	    {
		    UpdateSpawner();
		    yield return null;
	    }

	    yield return null;
	    OnPlantSpawned?.Invoke();
	    while (!Input.GetMouseButtonDown(0) || !IsValidRoot(targetRootPosition) || !GetCurrentPositions().Contains(targetRootPosition)){
		    UpdateRootSelect();
		    yield return null;
	    }
	    OnPlantCreated?.Invoke();
	    DrawWithPreviewPool(new HashSet<Vector2Int>());
    }

    bool ValidRootExists(HashSet<Vector2Int> plantPositions)
    {
	    return plantPositions.Any(IsValidRoot);
    }
    bool CurrentSpawnValid(HashSet<Vector2Int> plantPositions)
    {
		foreach (Vector2Int plantPosition in plantPositions){
			if (!WorldGrid.instance.CellTargetable(plantPosition)){
				return false;
			}
		}
	    return ValidRootExists(plantPositions);
    }
	void UpdateSpawner()
    {
        if(GameManager.instance.IsUIVisible()){return;}
	    int currentRotationAmount = rotationAmount;
	    if (Input.GetKey(KeyCode.LeftShift)){
			currentRotationAmount += (int)Input.mouseScrollDelta.y;
	    }
	    Vector2Int currentSpawnPosition = WorldToGrid(SceneCamera.GetWorldMousePosition());
	    if(currentSpawnPosition == targetSpawnPosition && currentRotationAmount == rotationAmount)
	    {
		    return;
	    }
	    targetSpawnPosition = currentSpawnPosition;
	    rotationAmount = currentRotationAmount;
	    
	    HashSet<Vector2Int> plantPositions = GetCurrentPositions();
	    bool validRootExists = ValidRootExists(plantPositions);
	    DrawWithPreviewPool(plantPositions, (pos) => {
		    if (!validRootExists){
			    return previewErrorColor;
		    }
			bool targetable = WorldGrid.instance.CellTargetable(pos);
			bool occupied = WorldGrid.instance.GetPlantAt(pos) != null;
			if (occupied)
			{
				return previewOccupiedColor;
			}
			return targetable ? previewColor : previewErrorColor;
		});
    }

	void UpdateRootSelect()
	{
        if(GameManager.instance.IsUIVisible()){return;}
		Vector2Int currentRootPosition = WorldToGrid(SceneCamera.GetWorldMousePosition());
		if(currentRootPosition == targetRootPosition)
		{
			return;
		}
		targetRootPosition = currentRootPosition;
		HashSet<Vector2Int> plantPositions = GetCurrentPositions();
		DrawWithPreviewPool(plantPositions, (pos) => {
			if (!IsValidRoot(pos)){
				return previewHiddenColor;
			}
			if(pos == targetRootPosition)
			{
				return previewHoverColor;
			}
			return previewColor;
		});
	}
	
    Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
    }

    HashSet<Vector2Int> GetCurrentPositions()
    {
	    HashSet<Vector2Int> plantPositions = plantGenerator(targetSpawnPosition);
	    int rotation = rotationAmount % 4;
	    if(rotation < 0)
	    {
		    rotation += 4;
	    }
	    for (int i = 0; i < rotation; i++){
		    plantPositions = RotateAround(targetSpawnPosition, plantPositions);
	    }
		plantPositions.Add(targetSpawnPosition);
		return plantPositions;
    }
    HashSet<Vector2Int> RotateAround(Vector2Int root, HashSet<Vector2Int> positions)
    {
	    HashSet<Vector2Int> rotated = new HashSet<Vector2Int>();
	    foreach (Vector2Int position in positions){
		    Vector2Int offset = position - root;
		    rotated.Add(new Vector2Int(offset.y, -offset.x) + root);
	    }

	    return rotated;
    }
	List<SpriteRenderer> previewPool = new();
	void DrawWithPreviewPool(HashSet<Vector2Int> positions, Func<Vector2Int,Color> getColor = null)
	{
		if(positions.Count > previewPool.Count)
		{
			ExpandPreviewPool(positions.Count - previewPool.Count);
		}
		if(getColor == null)
		{
			getColor = (pos) => {
				return previewColor;
			};
		}
		int previewIndex = 0;
		foreach (Vector2Int position in positions){
			SpriteRenderer previewSr = previewPool[previewIndex];
			Vector3 worldPos = (Vector2)position;
			worldPos.z = transform.position.z;
			previewSr.transform.position = worldPos;
			previewSr.enabled = true;
			previewSr.color = getColor(position);
			previewIndex++;

		}

		for (int i = positions.Count; i < previewPool.Count; i++){
			SpriteRenderer previewSr = previewPool[i];
			previewSr.enabled = false;
		}
		
	}
	void ExpandPreviewPool(int size)
	{
		for (int i = 0; i < size; i++){
			SpriteRenderer newSprite = Instantiate(previewSpritePrefab);
			previewPool.Add(newSprite);
		}
	}

	public HashSet<Vector2Int> GetPlantPositions()
	{
		return GetCurrentPositions();
	}

	public Vector2Int GetRootPosition()
	{
		return targetRootPosition;
	}
}
