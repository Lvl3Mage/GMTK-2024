using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlantRenderer : MonoBehaviour
{
    [SerializeField] PlantCellRenderer cellRendererPrefab;
    [ParentActionButton("TEST",nameof(Test), hideField:true)]
    [SerializeField] string plantName;
    [SerializeField] Vector2Int[] addCells;

    void Test()
    {
        FillCells(addCells);
    }

    void Update()
    {
        if (Input.GetMouseButton(0)){
            Vector2 mousePosition = SceneCamera.GetWorldMousePosition();
            Vector2Int cell = new Vector2Int((int)Mathf.Round(mousePosition.x), (int)Mathf.Round(mousePosition.y));
            if (!filledCells.Contains(cell)){
                FillCell(cell);
            }

    /// <summary>Returns an array with all children cell renderers the whole plant has. </summary>
    public PlantCellRenderer[] GetCellArray() => childRenderers.Values.ToArray();

    public void AddCell(Vector2Int cell)
    {
        AddCells(new Vector2Int[] { cell });
    }

    /// <summary>Recieves the cells of a single plant and renders them as tiles. </summary>
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
        pendingUpdates = updatedCells.ToList<Vector2Int>();

        if (doAnimations) StartBushShaking(); //It will call UpdateCells after waiting
        else UpdateCells();
    }

    /// <summary>Changes all pendingUpdates cells at once with a proper animation. </summary>
    public void UpdateCells()
    {
        foreach (Vector2Int cell in pendingUpdates)
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

                cellRenderer.SetTileData(neighbouredEdges);
            }
            else Debug.LogWarning("Trying to render an unexisting cell. ");
        }
    }
    
    /// <summary>Iterates over the renderers of all childs/cells and calls Destroy() on each of them. Then, it destroys itself. </summary>
    /// <param name="animate">Determines if an animation will be played when destroyed. </param>
    public void DestroyAll()
    {
        foreach (PlantCellRenderer cellRenderer in GetCellArray())
        {
            if (doAnimations) cellRenderer.PlayDestroyAnimation();
            else cellRenderer.Delete();
        }
        Destroy(gameObject);
    }

    private void StartBushShaking()
    {
        foreach (PlantCellRenderer cellRenderer in GetCellArray())
            cellRenderer.PlayShakeAnimation();
        
        StartCoroutine(TimedCellUpdate(shakeDuration)); //Wait some seconds and then update pending
    }

    private IEnumerator TimedCellUpdate(float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        UpdateCells();
    }
}
