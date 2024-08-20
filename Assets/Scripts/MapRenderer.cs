using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile[] land;
    [SerializeField] AnimatedTile water;

    readonly Dictionary<Vector2Int, MapCellType> mapData = new();
    readonly HashSet<Vector2Int> newTiles = new(); // Para almacenar las posiciones nuevas

    private void Start()
    {
    }

    public void FillBounds()
    {
        Vector2Int gridSize = WorldGrid.instance.GridSize;
        HashSet<Vector2Int> waterCells = WorldGrid.instance.GetWaterCells();
        Vector2Int origin = gridSize / 2;

        newTiles.Clear();
        for (int row = -origin.y; row <= gridSize.y / 2; row++)
        {
            for (int col = -origin.x; col <= gridSize.x / 2; col++)
            {
                Vector2Int coord = new(col, row);

                if (mapData.ContainsKey(coord))
                {
                    continue;
                }

                MapCellType cellType = waterCells.Contains(coord) ? MapCellType.Water : MapCellType.Land;
                mapData.Add(coord, cellType);
                newTiles.Add(coord);
            }
        }
    }

    public IEnumerator renderWorldGrid(float animationDuration = 0.5f)
    {
        foreach (var pair in mapData)
        {
            Vector3Int position = new Vector3Int(pair.Key.x, pair.Key.y, 0);
            MapCellType type = pair.Value;

            switch (type)
            {
                case MapCellType.Land:
                    if (Random.value < 0.01f)  // 1% de probabilidad
                    {
                        type = MapCellType.Water;
                        tileMap.SetTile(position, water);
                        tileMap.SetAnimationFrame(position, Random.Range(1, water.m_AnimatedSprites.Length + 1));
                        WorldGrid.instance.AddWaterPosition(pair.Key);
                    }
                    else
                    {
                        tileMap.SetTile(position, land[Random.Range(0, land.Length)]);
                    }
                    break;

                case MapCellType.Water:
                    tileMap.SetTile(position, water);
                    tileMap.SetAnimationFrame(position, Random.Range(1, water.m_AnimatedSprites.Length + 1));
                    break;

                default:
                    tileMap.SetTile(position, land[0]);
                    break;
            }

            if(!mapData.ContainsKey((Vector2Int)position))
            {
                tileMap.SetTransformMatrix(position, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero));
            }
        }


        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, elapsedTime / animationDuration);
            
            foreach (Vector2Int coord in newTiles)
            {
                Vector3Int position = new Vector3Int(coord.x, coord.y, 0);
                tileMap.SetTransformMatrix(position, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1)));
            }

            yield return null; 
        }

        foreach (Vector2Int coord in newTiles)
        {
            Vector3Int position = new Vector3Int(coord.x, coord.y, 0);
            tileMap.SetTransformMatrix(position, Matrix4x4.identity);
        }

        newTiles.Clear(); 
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(var pair in mapData)
        {
            Gizmos.DrawSphere((Vector2)pair.Key, 0.5f);
        }
    }
}

public enum MapCellType
{
    Land,
    Water
}
