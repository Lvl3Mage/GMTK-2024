#nullable enable
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Plant : MonoBehaviour
{
	
	public class WorldGrid
	{
		public static WorldGrid instance;
		public Plant? GetPlantAt(Vector2Int position)
		{
			throw new System.NotImplementedException();
		}
	}
	readonly HashSet<Vector2Int> growthPositions = new HashSet<Vector2Int>();
	readonly HashSet<Vector2Int> plantPositions = new HashSet<Vector2Int>();
	public void Create(Vector2Int[] positions, Vector2Int root)
	{
		foreach (Vector2Int position in positions){
			growthPositions.Add(position);
		}
	}
	void AddPlantPosition(Vector2Int position)
	{
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
		List<Plant> neighbours = new List<Plant>();
		Vector2Int[] cellNeighbours = CellUtils.GetCellNeighbours(position);
		foreach (Vector2Int neighbour in cellNeighbours){
			Plant? plant = WorldGrid.instance.GetPlantAt(neighbour);
			if (plant != null){
				neighbours.Add(plant);
			}
		}

		return neighbours.ToArray();
	}
	void RemoveDependency(Plant plant)
	{
		dependencies.Remove(plant);
	}
	public event Action<Plant> OnDestroyed;
	public void DestroyPlant()
	{
		OnDestroyed?.Invoke(this);
		Destroy(gameObject);
		//Todo remove from world grid
		//Todo remove from growth positions in world grid
	}
	
}
