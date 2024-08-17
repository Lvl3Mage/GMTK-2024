using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlantRenderer : MonoBehaviour
{
    Dictionary<Vector2Int, PlantCellRenderer> childRenderers = new(); //All 'child' bush cells the whole plant has
    [SerializeField] PlantCellRenderer cellRendererPrefab;

    /// <summary>
    /// Recieves the cells of a single plant and renders them as tiles. 
    /// </summary>
    /// <param name="cells">Each of the cell coordinates that the plant has. </param>
    public void Add(Vector2Int[] cells)
    {
        HashSet<Vector2Int> updatedCells = new(cells);

        foreach (Vector2Int cell in cells)
        {
            Vector2Int[] neighbours = CellUtils.GetCellNeighbours(cell);

            updatedCells.AddRange<Vector2Int>(neighbours);
            updatedCells.Add(cell);
            childRenderers.Add(cell, Instantiate(cellRendererPrefab, (Vector2)cell, Quaternion.identity));
        }

        UpdateSprites(updatedCells.ToList<Vector2Int>());
    }

    /// <summary>
    /// Changes the sprites of a given coordinate list, according to the state of WorldGrid. 
    /// </summary>
    /// <param name="cells">Each of the cell coordinates that will be updated. </param>
    public void UpdateSprites(List<Vector2Int> cells)
    {
        foreach (Vector2Int cell in cells)
        {
            //Update each cell
            PlantCellRenderer cellRenderer;

            if (childRenderers.ContainsKey(cell))
            {
                cellRenderer = childRenderers[cell];
                
                Vector2Int[] tileData = CellUtils.GetCellNeighbours(cell, true);
                bool[] neighbouredEdges = new bool[tileData.Length];
                
                for (int i = 0; i < tileData.Length; i++)
                {
                    neighbouredEdges[i] = WorldGrid.instance.GetPlantAt(tileData[i]) != null;
                }

                cellRenderer.SetTileData(neighbouredEdges);
            }
            else Debug.LogWarning("Trying to render an unexisting cell. ");
        }
    }
    
    /// <summary>
    /// Iterates over the renderers of all childs/cells and calls Destroy() on each of them. 
    /// </summary>
    /// <param name="animate">Determines if an animation will be played when destroyed. </param>
    public void Destroy(bool animate = true)
    {
        foreach (KeyValuePair<Vector2Int, PlantCellRenderer> pair in childRenderers)
        {
            pair.Value.Destroy(animate); //Each child manages its own animation
        }
    }
}
