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
    PlantGenerator plantGenerator = position => {
	    return new[]{ position};
    };


	Vector2Int targetRootPosition;
	

    public IEnumerator CreatePlant(PlantGenerator generator)
    {
	    plantGenerator = generator;
	    while(CanCreatePlant(targetRootPosition) && !Input.GetMouseButtonDown(0))
	    {
		    UpdateCreator();
		    yield return null;
	    }
    }
    bool CanCreatePlant(Vector2Int rootPosition)
	{
	    return WorldGrid.instance.CellTargetable(rootPosition);
	}
	void UpdateCreator()
    {
	    Vector2Int currentRootPosition = WorldToGrid(SceneCamera.GetWorldMousePosition());
	    if(currentRootPosition == targetRootPosition)
	    {
		    return;
	    }
	    targetRootPosition = currentRootPosition;
	    if(WorldGrid.instance.CellTargetable(targetRootPosition))
	    {
		    DrawWithPreviewPool(plantGenerator(targetRootPosition));
	    }
	    else
	    {
		    DrawWithPreviewPool(new[]{targetRootPosition}, ()=> previewErrorColor);
	    }
    }
    Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
    }

	List<SpriteRenderer> previewPool = new();
	void DrawWithPreviewPool(Vector2Int[] positions, Func<Color> getColor = null)
	{
		if(positions.Length > previewPool.Count)
		{
			ExpandPreviewPool(positions.Length - previewPool.Count);
		}
		if(getColor == null)
		{
			getColor = () => {
				bool targetable = WorldGrid.instance.CellTargetable(targetRootPosition);
				return targetable ? previewColor : previewErrorColor;
			};
		}

		for (int i = 0; i < positions.Length; i++){
			SpriteRenderer previewSr = previewPool[i];
			previewSr.transform.position = (Vector2)positions[i];
			previewSr.enabled = true;
			previewSr.color = getColor();
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
