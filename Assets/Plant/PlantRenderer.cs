using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlantRenderer : MonoBehaviour
{
    Dictionary<Vector2Int, PlantCellRenderer> childRenderers = new(); //All 'child' cells the whole plant currently has
    [SerializeField] PlantCellRenderer cellRendererPrefab;

    public void AddCell(Vector2Int cell)
    {
        AddCells(new Vector2Int[] { cell });
    }
    /// <summary>
    /// Recieves the cells of a single plant and renders them as tiles. 
    /// </summary>
    /// <param name="cells">Each of the cell coordinates that the plant has. </param>
    public void AddCells(Vector2Int[] cells)
    {
        //updatedCells will store the cells where bushes will be added + cells that will change due to those bushes
        HashSet<Vector2Int> updatedCells = new(cells);

        foreach (Vector2Int cell in cells)
        {
            Vector2Int[] neighbours = CellUtils.GetCellNeighbours(cell);

            updatedCells.AddRange<Vector2Int>(neighbours); //Add neighbours
            updatedCells.Add(cell); //Add itself
            childRenderers.Add(cell, Instantiate(cellRendererPrefab, (Vector2)cell, Quaternion.identity));
        }

        UpdateSprites(updatedCells.ToList<Vector2Int>()); //Modify all cell tiles that need to be changed
    }

    void UpdateSprites(List<Vector2Int> cells)
    {
        // Changes the sprites of a given coordinate (cell) list, according to the state of WorldGrid. 
        foreach (Vector2Int cell in cells)
        {
            //Update each cell
            PlantCellRenderer cellRenderer;

            if (childRenderers.ContainsKey(cell)) //Guaranteed to happen
            {
                cellRenderer = childRenderers[cell];
                Vector2Int[] tileData = CellUtils.GetCellNeighbours(cell, true);
                bool[] neighbouredEdges = new bool[tileData.Length]; //tileData.Length is always 9
                
                for (int i = 0; i < tileData.Length; i++)
                {
                    //WorldGrid.instance.GetPlantAt(tileData[i]) != null; (use to make cells combine even if they come from different plants)
                    neighbouredEdges[i] = childRenderers.ContainsKey(tileData[i]); //cells only take into account other cells from their plant
                }

                cellRenderer.SetTileEdges(neighbouredEdges);
                cellRenderer.TriggerAnimation();
            }
            else Debug.LogWarning("Trying to render an unexisting cell. ");
        }
    }
    
    /// <summary>
    /// Iterates over the renderers of all childs/cells and calls Destroy() on each of them. Then, it destroys itself. 
    /// </summary>
    /// <param name="animate">Determines if an animation will be played when destroyed. </param>
    public void DestroyAll(bool animate = true)
    {
        foreach (KeyValuePair<Vector2Int, PlantCellRenderer> pair in childRenderers)
        {
            pair.Value.DestroyCell(animate); //Each child manages its own animation
        }
        Destroy(gameObject);
    }
}
