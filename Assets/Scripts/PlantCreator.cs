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
    PlantGenerator plantGenerator = (position) => {
	    HashSet<Vector2Int> points = new HashSet<Vector2Int>();
	    points.Add(position);
	    return points;
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
	    DrawWithPreviewPool(new HashSet<Vector2Int>());
    }
    bool CanCreatePlant(Vector2Int rootPosition)
	{
		HashSet<Vector2Int> plantPositions = plantGenerator(rootPosition);
		foreach (Vector2Int plantPosition in plantPositions){
			if (!WorldGrid.instance.CellTargetable(plantPosition)){
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
	void DrawWithPreviewPool(HashSet<Vector2Int> positions, Func<Vector2Int,Color> getColor = null)
	{
		if(positions.Count > previewPool.Count)
		{
			ExpandPreviewPool(positions.Count - previewPool.Count);
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
		HashSet<Vector2Int> plantPositions = plantGenerator(targetRootPosition);
		plantPositions.Add(targetRootPosition);
		return plantPositions;
	}

	public Vector2Int GetRootPosition()
	{
		return targetRootPosition;
	}
}
