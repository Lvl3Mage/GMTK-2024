﻿using System;
using UnityEngine;


public static class CellUtils
{
	public static Vector2Int[] GetCellNeighbours(Vector2Int cell, bool includeCenter = false)
	{
		
		Vector2Int[] neighbours = new Vector2Int[includeCenter ? 9 : 8];
		
		int i = 0;
		for (int y = -1; y <= 1; y++){
			for (int x = -1; x <= 1; x++){
				if (x == 0 && y == 0 && !includeCenter) continue;
				neighbours[i] = cell + new Vector2Int(x, y);
				i++;	
			}
		}

		return neighbours;
	}
	public static Vector2Int[] GetCellQuadrant(Vector2Int cell)
	{
		return new[]{
			cell,
			cell + Vector2Int.right,
			cell + Vector2Int.up,
			cell + Vector2Int.one,
		};
	}
	public static Vector2Int[] CellQuadrantOffsets()
	{
		return new[]{
			Vector2Int.zero,
			Vector2Int.right,
			Vector2Int.up,
			Vector2Int.one,
		};
	}
}