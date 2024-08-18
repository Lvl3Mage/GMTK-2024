using UnityEngine;

public class PlantCellRenderer : MonoBehaviour
{
    [SerializeField] Renderer tileRenderer;
    [SerializeField] Animator animator;
    bool[] currentVertices = new bool[4];
    int GetTileIndex(bool[] edges)
    {
        int index = 0;
        for (int i = edges.Length-1; i >= 0; i--){
            index <<= 1;
            index += edges[i] ? 1 : 0;
        }

        return index;
    }

    // int[] GetTileIndices(bool[] edges)
    // {
    //     bool[][] subTiles = 
    //     {
    //         new[]{edges[0],edges[1],edges[3],edges[4]},
    //         new[]{edges[1],edges[2],edges[4],edges[5]},
    //         new[]{edges[3],edges[4],edges[6],edges[7]},
    //         new[]{edges[4],edges[5],edges[7],edges[8]}
    //     };
    //     for (int i = 0; i < 4; i++){
    //         bool filled = true;
    //         for (int j = 0; j < 4; j++){
    //             filled &= subTiles[i][j];
    //         }
    //     
    //         subTiles[i][i] = filled;
    //     }
    //     int[] indices = new int[]{
    //         GetSubTileIndex(subTiles[0]),
    //         GetSubTileIndex(subTiles[1]),
    //         GetSubTileIndex(subTiles[2]),
    //         GetSubTileIndex(subTiles[3]),
    //     };
    //
    //     return indices;
    // }
    public void Refresh(bool[] vertices, Color[] vertexColors)
    {
        currentVertices = vertices;
        int index = GetTileIndex(vertices);
        tileRenderer.material.SetFloat("_TileIndex", index);
        tileRenderer.material.SetColor("_TintBottomLeft", vertexColors[0]);
        tileRenderer.material.SetColor("_TintBottomRight", vertexColors[1]);
        tileRenderer.material.SetColor("_TintTopLeft", vertexColors[2]);
        tileRenderer.material.SetColor("_TintTopRight", vertexColors[3]);
        
    }

    public void Destroy(bool animate = true)
    {
        if (animate)
        {
            //animator
        }
    }

    void OnDrawGizmos()
    {
        Vector2Int position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int[] neighbours = CellUtils.GetCellQuadrant(position);
        
        for (int i = 0; i < neighbours.Length; i++)
        {
            Vector2Int neighbour = neighbours[i];
            Gizmos.color = currentVertices[i] ? Color.green : Color.red;
            Gizmos.DrawWireSphere((Vector2)neighbour, currentVertices[i] ? 0.5f : 0.25f);
        }
    }
}
