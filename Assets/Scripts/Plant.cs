#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Plant : MonoBehaviour
{
	[SerializeField] Color plantColor;
	public Color PlantColor => plantColor;
	readonly HashSet<Vector2Int> growthPositions = new();
	readonly HashSet<Vector2Int> plantPositions = new();
	Vector2Int rootPosition;
	public void Create(Vector2Int[] positions, Vector2Int root)
	{
		foreach (Vector2Int position in positions){
			growthPositions.Add(position);
		}
		WorldGrid.instance.AddGrowthPositions(growthPositions.ToArray());
		rootPosition = root;
		// AddPlantPosition(root);
	}

	public void Grow()
	{
		HashSet<Vector2Int> growthTargets = new HashSet<Vector2Int>();
		growthTargets.Add(rootPosition);
		foreach (Vector2Int plantPosition in plantPositions){
			Vector2Int[] cellNeighbours = CellUtils.GetTrueCellNeighbours(plantPosition);
			growthTargets.AddRange(cellNeighbours);
		}
		growthTargets.ExceptWith(plantPositions);
		growthTargets.IntersectWith(growthPositions);
		foreach (Vector2Int growthTarget in growthTargets){
			Debug.Log($"Growing to {growthTarget}");
			AddPlantPosition(growthTarget);
		}
	}
	void AddPlantPosition(Vector2Int position)
	{
		WorldGrid.instance.RegisterPlant(position, this);
		plantPositions.Add(position);
		Plant[] neighbours = GetPlantNeighbours(position);
		foreach (Plant neighbour in neighbours){
			neighbour.AddDependency(this);
			AddDependency(neighbour);
		}
		
	}
	readonly HashSet<Plant> dependencies = new HashSet<Plant>();
	public void AddDependency(Plant plant)
	{
		dependencies.Add(plant);
		plant.OnDestroyed += RemoveDependency;
	}
	Plant[] GetPlantNeighbours(Vector2Int position)
	{
		Vector2Int[] cellNeighbours = CellUtils.GetCellNeighbours(position);

		return cellNeighbours.Select(neighbour => WorldGrid.instance.GetPlantAt(neighbour)).OfType<Plant>().ToArray();
	}
	void RemoveDependency(Plant plant)
	{
		dependencies.Remove(plant);
	}
	public delegate void PlantDeletionHandler(Plant plant);
	public event PlantDeletionHandler OnDestroyed;
	public void DestroyPlant()
	{
		OnDestroyed?.Invoke(this);
		Destroy(gameObject);
		foreach (Vector2Int plantPosition in plantPositions){
			WorldGrid.instance.RemovePlantAt(plantPosition);
		}
		WorldGrid.instance.RemoveGrowthPositions(growthPositions.ToArray());
	}
	
}
