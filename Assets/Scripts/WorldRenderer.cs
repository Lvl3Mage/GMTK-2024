using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WorldRenderer : MonoBehaviour
{
    public static WorldRenderer instance { get; private set; }
    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another instance of WorldRenderer exists!");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    [SerializeField] PlantCellRenderer plantCellRendererPrefab;
    
    
    Dictionary<Vector2Int, PlantCellRenderer> rendererGrid = new();
    HashSet<Vector2Int> filledCells = new();
    HashSet<Vector2Int> pendingRendererUpdates = new();
    Dictionary<Vector2Int,Color> filledCellColors = new();

    void GenerateRenderGrid(HashSet<Vector2Int> generationPositions)
    {
        HashSet<Vector2Int> spawnPositions = new(generationPositions);
        spawnPositions.ExceptWith(rendererGrid.Keys);
        SpawnRenderers(spawnPositions.ToArray());
        
    }
    void SpawnRenderers(Vector2Int[] positions)
    {
        foreach (Vector2Int position in positions){
            Vector2 worldPosition = GetWorldTilePosition(position);
            PlantCellRenderer plantCellRenderer = Instantiate(plantCellRendererPrefab, worldPosition, Quaternion.identity);
            rendererGrid.Add(position, plantCellRenderer);
        }
    }
    public Vector2 GetWorldTilePosition(Vector2Int position)
    {
        return new Vector2(position.x, position.y) + Vector2.one * 0.5f;
    }
    public class FillGroup
    {
        public FillGroup(HashSet<Vector2Int> positions, Color color)
        {
            this.positions = positions;
            this.color = color;
        }
        public HashSet<Vector2Int> positions = new();
        public Color color;
    }
    public void AddFilledCells(FillGroup[] fillGroups)
    {
        foreach (FillGroup fillGroup in fillGroups){
            AddFilledCells(fillGroup.positions, fillGroup.color);
        }
    }
    public void AddFilledCells(HashSet<Vector2Int> positionsToFill, Color color)
    {
        filledCells.UnionWith(positionsToFill);
        foreach (Vector2Int pos in positionsToFill){
            filledCellColors[pos] = color;
        }
        PropagateFillChanges(positionsToFill);
        
    }
    public void RemoveFilledCells(HashSet<Vector2Int> positionsToRemove)
    {
        filledCells.ExceptWith(positionsToRemove);
        foreach (Vector2Int pos in positionsToRemove){
            filledCellColors.Remove(pos);
        }
        PropagateFillChanges(positionsToRemove);
    }
    
    void PropagateFillChanges(HashSet<Vector2Int> fillPositions)
    { 
        HashSet<Vector2Int> affectedRendererPositions = new(fillPositions);
        foreach (Vector2Int updatedPosition in fillPositions){
            Vector2Int[] neighbours = CellUtils.GetCellQuadrant(updatedPosition - Vector2Int.one);
            foreach (Vector2Int neighbour in neighbours){
                Debug.Log($"Adding neighbour {neighbour}");
            }
            affectedRendererPositions.UnionWith(neighbours);
        }
        RefreshRendererData(affectedRendererPositions);
        
        pendingRendererUpdates.UnionWith(affectedRendererPositions);
    }
    void RefreshRendererData(HashSet<Vector2Int> affectedRenderers)
    {
        GenerateRenderGrid(affectedRenderers);
        foreach (Vector2Int affectedRenderer in affectedRenderers){
            PlantCellRenderer plantCellRenderer = rendererGrid[affectedRenderer];
            
            bool[] quadrantFillData = new bool[4];
            Vector2Int[] quadrantPositions = CellUtils.GetCellQuadrant(affectedRenderer);
            
            for (int i = 0; i < quadrantPositions.Length; i++){
                quadrantFillData[i] = filledCells.Contains(quadrantPositions[i]);
            }

            Color averageColor = GetAverageColor(quadrantPositions);
            Color[] quadrantColors = new Color[4];
            for (int i = 0; i < quadrantColors.Length; i++){
                if(filledCellColors.ContainsKey(quadrantPositions[i])){
                    quadrantColors[i] = filledCellColors[quadrantPositions[i]];
                }
                else
                {
                    quadrantColors[i] = averageColor;
                }
            }
            
            plantCellRenderer.SetData(quadrantFillData, quadrantColors);
        }
    }
    Color GetAverageColor(Vector2Int[] positions)
    {
        Color averageColor = new Color(0,0,0);
        int filledColorsCount = 0;
        for (int i = 0; i < positions.Length; i++){
            if (!filledCellColors.ContainsKey(positions[i])) continue;
            
            
            averageColor += filledCellColors[positions[i]];
            filledColorsCount++;
        }
        averageColor /= filledColorsCount;
        return averageColor;
    }
    
    //Animation
    public void InitiateShake()
    {
        foreach (Vector2Int pendingRendererUpdate in pendingRendererUpdates){
            rendererGrid[pendingRendererUpdate].AnimateShake();
        }
    }
    public void InitiateSpriteChange()
    {
        foreach (Vector2Int pendingRendererUpdate in pendingRendererUpdates){
            rendererGrid[pendingRendererUpdate].AnimateSpriteChange();
        }
        pendingRendererUpdates.Clear();
    }
}
