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
            Debug.LogWarning("Another instance of WorldRenderer exists!"); // si existiese otro WorldRenderer se autodestruiria
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    Dictionary<Vector2Int, PlantCellRenderer> renderers = new();
    HashSet<Vector2Int> filledCells = new();
    [SerializeField] PlantCellRenderer plantCellRendererPrefab;
    Dictionary<Vector2Int,Color> filledCellColors = new();
    public void FillCells(Vector2Int[] positionsToFill, Color color)
    {
        filledCells.UnionWith(positionsToFill);
        foreach (Vector2Int pos in positionsToFill){
            filledCellColors[pos] = color;
        }
        HashSet<Vector2Int> affectedRendererPositions = new(positionsToFill);
        foreach (Vector2Int positionToFill in positionsToFill){
            Vector2Int[] neighbours = CellUtils.GetCellNeighbours(positionToFill);
            affectedRendererPositions.UnionWith(neighbours);
        }
        HashSet<Vector2Int> spawnPositions = new(affectedRendererPositions.Except(renderers.Keys));
        
        SpawnRenderers(spawnPositions.ToArray());
        RefreshRenderers(affectedRendererPositions);
        
    }
    void SpawnRenderers(Vector2Int[] positions)
    {
        foreach (Vector2Int position in positions){
            PlantCellRenderer plantCellRenderer = Instantiate(plantCellRendererPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            renderers.Add(position, plantCellRenderer);
        }
    }
    void RefreshRenderers(HashSet<Vector2Int> affectedRenderers)
    {
        foreach (Vector2Int affectedRenderer in affectedRenderers){
            
            if(!renderers.ContainsKey(affectedRenderer))
            {
                Debug.LogWarning("Trying to refresh a non-existent renderer");
                continue;
            }
            PlantCellRenderer plantCellRenderer = renderers[affectedRenderer];
            
            bool[] surroundingFill = new bool[4];
            Vector2Int[] surroundingPositions = CellUtils.GetCellQuadrant(affectedRenderer);
            for (int i = 0; i < surroundingPositions.Length; i++){
                surroundingFill[i] = filledCells.Contains(surroundingPositions[i]);
            }

            int filledColorsCount = 0;
            Color averageColor = new Color(0,0,0);
            for (int i = 0; i < surroundingPositions.Length; i++){
                if (filledCellColors.ContainsKey(surroundingPositions[i])){
                    averageColor += filledCellColors[surroundingPositions[i]];
                    filledColorsCount++;
                }
            }
            averageColor /= filledColorsCount;
            Color[] filledColors = new Color[4];
            for (int i = 0; i < filledColors.Length; i++){
                if(filledCellColors.ContainsKey(surroundingPositions[i])){
                    filledColors[i] = filledCellColors[surroundingPositions[i]];
                }
                else
                {
                    filledColors[i] = averageColor;
                }
            }
            
            
            plantCellRenderer.Refresh(surroundingFill, filledColors);
            
            
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
