#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Plant : MonoBehaviour
{
	Color plantColor;
	public Color PlantColor => plantColor;
	readonly HashSet<Vector2Int> growthPositions = new();
	readonly HashSet<Vector2Int> plantPositions = new();
	Vector2Int rootPosition;
	bool plantDestroyed = false;
	[SerializeField] GameObject seedPrefab;
	[SerializeField] float seedAnimationDuration;
	[SerializeField] float seedZPosition;
	[SerializeField] Color[] colorBuffer;
	
	static int colorBufferIndex = 0;
	public IEnumerator Create(HashSet<Vector2Int> positions, Vector2Int root)
	{
		colorBufferIndex++;
		if (colorBufferIndex >= colorBuffer.Length){
			colorBufferIndex = 0;
		}
		plantColor = colorBuffer[colorBufferIndex];
		growthPositions.UnionWith(positions);
		WorldGrid.instance.AddGrowthPositions(growthPositions.ToArray());
		rootPosition = root;
		Vector3 position = new Vector3(rootPosition.x, rootPosition.y,seedZPosition);
		GameObject seedObj = Instantiate(seedPrefab, position, Quaternion.identity);
		yield return new WaitForSeconds(seedAnimationDuration);
		Destroy(seedObj);
	}

	public HashSet<Vector2Int> Grow()
	{
		HashSet<Vector2Int> growthTargets = new(){ rootPosition };
		
		foreach (Vector2Int plantPosition in plantPositions){
			Vector2Int[] cellNeighbours = CellUtils.GetTrueCellNeighbours(plantPosition);
			growthTargets.AddRange(cellNeighbours);
		}
		growthTargets.ExceptWith(plantPositions);
		growthTargets.IntersectWith(growthPositions);

		VerifyGrowthTargets(growthTargets);
		//Plant can destroy itself while growing
		if (plantDestroyed){
			return new HashSet<Vector2Int>();
		}
		
		foreach (Vector2Int growthTarget in growthTargets){
			GrowTo(growthTarget);
		}

		return growthTargets;
	}

	void VerifyGrowthTargets(HashSet<Vector2Int> growthTargets)
	{
		foreach (Vector2Int growthTarget in growthTargets){
			Plant? otherPlant = WorldGrid.instance.GetPlantAt(growthTarget);
			if(otherPlant != null && otherPlant != this){
				Debug.Log($"Destroying at {growthTarget}");
				PlantManager.instance.DestroyPlant(otherPlant);
				PlantManager.instance.DestroyPlant(this);
			}
		}
	}
	void GrowTo(Vector2Int position)
	{
		WorldGrid.instance.RegisterPlant(position, this);
		plantPositions.Add(position);
		Plant[] cellNeighbours = GetNeighboursAtCell(position);
		foreach (Plant neighbour in cellNeighbours){
			neighbour.AddNeighbour(this);
			AddNeighbour(neighbour);
		}
		
	}
	readonly HashSet<Plant> neighbours = new();
	public HashSet<Plant> GetNeighbours()
	{
		return new HashSet<Plant>(neighbours);
	}
	
	
	void AddNeighbour(Plant plant)
	{
		neighbours.Add(plant);
	}
	void RemoveNeighbour(Plant plant)
	{
		neighbours.Remove(plant);
	}
	Plant[] GetNeighboursAtCell(Vector2Int position)
	{
		Vector2Int[] cellNeighbours = CellUtils.GetCellNeighbours(position);

		return cellNeighbours.Select(neighbour => WorldGrid.instance.GetPlantAt(neighbour)).OfType<Plant>().ToArray();
	}
	public void DestroyPlant()
	{
		if (plantDestroyed){
			return;
		}

		plantDestroyed = true;
		Plant[] neighboursArr = neighbours.ToArray();

		for (int i = 0; i < neighboursArr.Length; i++){
			Plant neighbour = neighboursArr[i];
			neighbour.RemoveNeighbour(this);
		}

		foreach (Vector2Int plantPosition in plantPositions){
			WorldGrid.instance.RemovePlantAt(plantPosition);
		}
		WorldGrid.instance.RemoveGrowthPositions(growthPositions);
        SoundController.instance.PlayDestoyingBush();
        Destroy(gameObject);
	}
}
