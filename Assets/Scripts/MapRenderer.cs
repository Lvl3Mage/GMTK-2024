using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    [SerializeField]Tilemap tileMap;
    [SerializeField] Tile[] land;
    [SerializeField] AnimatedTile water;

    Dictionary<Vector2Int, MapCellType> mapData = new();

    private void Start()
    {
    }

    public void FillBounds()
    {
        Vector2Int gridSize = new Vector2Int(5, 3);//WorldGrid.instance.GridSize; //Always odd sizes
        HashSet<Vector2Int> waterCells = WorldGrid.instance.GetWaterCells();
        Vector2Int origin = gridSize / 2;

        for (int row = -origin.y; row <= gridSize.y / 2; row++)
            for (int col = -origin.x; col <= gridSize.x / 2; col++)
            {
                Vector2Int coord = new(col, row);
                print(coord);
                MapCellType cellType;

                if (waterCells.Contains(coord))
                {
                    cellType = MapCellType.Water;
                }
                else cellType = MapCellType.Land;

                mapData.Add(coord, cellType);
            }
    }

    public void renderWorldGrid()
    {
        foreach (var pair in mapData)
        {
            Vector3Int position = new Vector3Int(pair.Key.x, pair.Key.y, 0);
            MapCellType type = pair.Value;

            switch (type)
            {
                case MapCellType.Land:
                    tileMap.SetTile(position, land[Random.Range(0, land.Length)]);
                    break;
                case MapCellType.Water:
                    tileMap.SetTile(position, water);
                    tileMap.SetAnimationFrame(position, Random.Range(1, water.m_AnimatedSprites.Length + 1));
                    break;
                default:
                    tileMap.SetTile(position, land[0]);
                    break;
            }
        }
    }
}

public enum MapCellType
{
    Land,
    Water
}
