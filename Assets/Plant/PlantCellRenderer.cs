using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlantCellRenderer : MonoBehaviour
{
    [SerializeField] Renderer[] tileRenderers;
    [SerializeField] Animator animator;
    bool[] currentEdges = new bool[9];
    int GetSubTileIndex(bool[] edges)
    {
        int index = 0;
        for (int i = edges.Length-1; i >= 0; i--){
            index <<= 1;
            index += edges[i] ? 1 : 0;
        }

        return index;
    }

    int[] GetTileIndices(bool[] edges)
    {
        bool[][] subTiles = 
        {
            new[]{edges[0],edges[1],edges[3],edges[4]},
            new[]{edges[1],edges[2],edges[4],edges[5]},
            new[]{edges[3],edges[4],edges[6],edges[7]},
            new[]{edges[4],edges[5],edges[7],edges[8]}
        };
        for (int i = 0; i < 4; i++){
            bool filled = true;
            for (int j = 0; j < 4; j++){
                filled &= subTiles[i][j];
            }
        
            subTiles[i][i] = filled;
        }
        int[] indices = new int[]{
            GetSubTileIndex(subTiles[0]),
            GetSubTileIndex(subTiles[1]),
            GetSubTileIndex(subTiles[2]),
            GetSubTileIndex(subTiles[3]),
        };

        return indices;
    }

    public void SetTileData(bool[] edges)
    {
        currentEdges = edges;
        int[] indices = GetTileIndices(edges);
        for (int i = 0; i < tileRenderers.Length; i++){
            tileRenderers[i].material.SetInt("_TileIndex", indices[i]);
        }
        //Update shaders
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
        Vector2Int[] neighbours = CellUtils.GetCellNeighbours(position, true);
        
        for (int i = 0; i < neighbours.Length; i++)
        {
            Vector2Int neighbour = neighbours[i];
            Gizmos.color = currentEdges[i] ? Color.green : Color.red;
            Gizmos.DrawWireSphere((Vector2)neighbour, 0.1f);
        }
    }
}
