using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lvl3Mage.CameraManagement2D;
using UnityEngine;

public class PlantCreator : MonoBehaviour
{
    [SerializeField] SpriteRenderer previewSpritePrefab;
    [SerializeField] Color previewColor;
    [SerializeField] Color previewErrorColor;
    [SerializeField] Color previewOccupiedColor;
    PlantGenerator plantGenerator = position => {
	    return new[]{ position};
    };


	Vector2Int targetRootPosition;
	

    public IEnumerator CreatePlant(PlantGenerator generator)
    {
	    plantGenerator = generator;
	    while(!Input.GetMouseButtonDown(0) || !CanCreatePlant(targetRootPosition))
	    {
		    UpdateCreator();
		    yield return null;
	    }
	    DrawWithPreviewPool(Array.Empty<Vector2Int>());
    }
    bool CanCreatePlant(Vector2Int rootPosition)
	{
		Vector2Int[] plantPositions = plantGenerator(rootPosition);
		foreach (Vector2Int plantPosition in plantPositions){
			if (!WorldGrid.instance.CellTargetable(rootPosition)){
				return false;
			}
		}
	    return WorldGrid.instance.CellTargetable(rootPosition) && WorldGrid.instance.GetPlantAt(rootPosition) == null;
	}
	void UpdateCreator()
    {
	    Vector2Int currentRootPosition = WorldToGrid(SceneCamera.GetWorldMousePosition());
	    if(currentRootPosition == targetRootPosition)
	    {
		    return;
	    }
	    targetRootPosition = currentRootPosition;
	    DrawWithPreviewPool(plantGenerator(targetRootPosition));
    }
    Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
    }

	List<SpriteRenderer> previewPool = new();
	void DrawWithPreviewPool(Vector2Int[] positions, Func<Vector2Int,Color> getColor = null)
	{
		if(positions.Length > previewPool.Count)
		{
			ExpandPreviewPool(positions.Length - previewPool.Count);
		}
		if(getColor == null)
		{
			getColor = (pos) => {
				bool targetable = WorldGrid.instance.CellTargetable(pos);
				bool occupied = WorldGrid.instance.GetPlantAt(pos) != null;
				if (occupied)
				{
					return previewOccupiedColor;
				}
				return targetable ? previewColor : previewErrorColor;
			};
		}

		for (int i = 0; i < positions.Length; i++){
			SpriteRenderer previewSr = previewPool[i];
			Vector3 position = (Vector2)positions[i];
			position.z = transform.position.z;
			previewSr.transform.position = position;
			previewSr.enabled = true;
			previewSr.color = getColor(positions[i]);
		}

		for (int i = positions.Length; i < previewPool.Count; i++){
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

	public Vector2Int[] GetPlantPositions()
	{
		return plantGenerator(targetRootPosition);
	}

	public Vector2Int GetRootPosition()
	{
		return targetRootPosition;
	}
}
