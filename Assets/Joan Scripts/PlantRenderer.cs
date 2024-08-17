using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlantRenderer : MonoBehaviour
{
    public Dictionary<Vector2Int, PlantCellRenderer> renderers = new();
    [SerializeField] PlantCellRenderer cellRendererPrefab; 

    public void Add(Vector2Int[] cells)
    {
        HashSet<Vector2Int> updatedCells = new(cells);

        foreach (Vector2Int cell in cells)
        {
            Vector2Int[] neighbours = CellUtils.GetCellNeighbours(cell);

            updatedCells.AddRange<Vector2Int>(neighbours);
            updatedCells.Add(cell);
            renderers.Add(cell, Instantiate(cellRendererPrefab, (Vector2)cell, Quaternion.identity));
        }

        UpdateSprites(updatedCells.ToList<Vector2Int>());
    }

    public void UpdateSprites(List<Vector2Int> cells)
    {
        foreach (Vector2Int cell in cells)
        {
            int spriteIndex = GetTileIndex(cell);
            //cell.asignTheFokinSprite(spriteIndex);
        }
    }

    public int GetTileIndex(Vector2Int cell)
    {
        //if water return 0
        //if plant return 1...
        return 0;
    }
}
