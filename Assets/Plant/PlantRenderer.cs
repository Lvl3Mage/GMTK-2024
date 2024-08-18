using System.Collections.Generic;
using System.Linq;
using Lvl3Mage.CameraManagement2D;
using Lvl3Mage.EditorEnhancements.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlantRenderer : MonoBehaviour
{
    Dictionary<Vector2Int, PlantCellRenderer> childRenderers = new(); //All 'child' bush cells the whole plant has
    HashSet<Vector2Int> filledCells = new();
    [SerializeField] PlantCellRenderer cellRendererPrefab;
    [ParentActionButton("Change Color",nameof(Test), hideField:true)]
    [SerializeField] string plantName;
    [SerializeField] Vector2Int[] addCells;
    [SerializeField] Color color;
    Color originalColor;
    void Test()
    {
        originalColor = color;
        color = originalColor;
        color.r += Random.Range(-0.1f,0.1f);
        color.g += Random.Range(-0.1f,0.1f);
        color.b += Random.Range(-0.1f,0.1f);
    }

    void Update()
    {
        if (Input.GetMouseButton(0)){
            Vector2 mousePosition = SceneCamera.GetWorldMousePosition();
            Vector2Int cell = new Vector2Int((int)Mathf.Round(mousePosition.x), (int)Mathf.Round(mousePosition.y));
            if (!filledCells.Contains(cell)){
                filledCells.Add(cell);
                WorldRenderer.instance.FillCells(new Vector2Int[]{cell},color);
            }

        }
    }

    public void FillCell(Vector2Int cell)
    {
        FillCells(new Vector2Int[] { cell });
    }
    /// <summary>
    /// Recieves the cells of a single plant and renders them as tiles. 
    /// </summary>
    /// <param name="cells">Each of the cell coordinates that the plant has. </param>
    public void FillCells(Vector2Int[] cells)
    {
        filledCells.UnionWith(cells);
        HashSet<Vector2Int> cellsToAdd = new(cells);
        
        // foreach (Vector2Int cell in cells)
        // {
        //     Vector2Int[] neighbours = CellUtils.GetCellNeighbours(cell);
        //     cellsToAdd.AddRange<Vector2Int>(neighbours);
        // }
        HashSet<Vector2Int> existingCells = new(childRenderers.Keys);
        cellsToAdd.ExceptWith(existingCells);
        
        
        foreach (Vector2Int newRenderingCell in cellsToAdd){
            PlantCellRenderer cellRenderer = Instantiate(cellRendererPrefab, (Vector2)newRenderingCell, Quaternion.identity);
            childRenderers.Add(newRenderingCell, cellRenderer);
        }
        
        
        HashSet<Vector2Int> updatedCells = new(cellsToAdd);

        foreach (Vector2Int cell in cellsToAdd)
        {
            Vector2Int[] neighbours = CellUtils.GetCellNeighbours(cell);
            updatedCells.AddRange(neighbours);
        }

        UpdateSprites(updatedCells.ToArray());
    }

    /// <summary>
    /// Changes the sprites of a given coordinate list, according to the state of WorldGrid. 
    /// </summary>
    /// <param name="cells">Each of the cell coordinates that will be updated. </param>
    public void UpdateSprites(Vector2Int[] cells)
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
                    neighbouredEdges[i] = filledCells.Contains(tileData[i]);
                }

                // cellRenderer.SetTileData(neighbouredEdges);
            }
            else Debug.LogWarning("Trying to render a non-existing cell. ");
        }
    }
    
    /// <summary>
    /// Iterates over the renderers of all childs/cells and calls Destroy() on each of them. 
    /// </summary>
    /// <param name="animate">Determines if an animation will be played when destroyed. </param>
    public void DestroyCells(bool animate = true)
    {
        foreach (KeyValuePair<Vector2Int, PlantCellRenderer> pair in childRenderers)
        {
            pair.Value.Destroy(animate); //Each child manages its own animation
        }
    }
}
