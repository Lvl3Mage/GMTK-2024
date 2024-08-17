using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlantCellRenderer : MonoBehaviour
{
    [SerializeField] Renderer tileRenderer;
    [SerializeField] Animator animator;
    public int GetSubTileIndex(bool[] edges)
    {
        int index = 0;
        foreach (bool edge in edges){
            index += edge ? 1 : 0;
            index <<= 1;
        }

        return index;
    }

    int[] GetTileIndecies(bool[] edges)
    {
        int [] indices = new int[]{
            GetSubTileIndex(new bool[]{edges[0],edges[1],edges[3],edges[4]}),
            GetSubTileIndex(new bool[]{edges[1],edges[2],edges[4],edges[5]}),
            GetSubTileIndex(new bool[]{edges[3],edges[4],edges[6],edges[7]}),
            GetSubTileIndex(new bool[]{edges[4],edges[5],edges[7],edges[8]}),
        };

        return indices;
    }

    public void SetTileData(bool[] edges)
    {
        int[] indices = GetTileIndecies(edges);
        Vector4 indexData = new Vector4(indices[0], indices[1], indices[2], indices[3]);
        tileRenderer.material.SetVector("_TileIndex", indexData);
        //Update shaders
    }

    public void Destroy(bool animate = true)
    {
        if (animate)
        {
            //animator
        }
    }
}
