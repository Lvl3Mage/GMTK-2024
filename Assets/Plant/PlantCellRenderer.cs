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

    bool[] edges = new bool[8]; //true = edge is on collision | false = it is not
    bool alreadySpawned = false; //Makes an spawn animation play if updated for the first time

    /// <summary>
    /// Modifies the whole cell edges. Each PlantRenderer uses it to define its childs. 
    /// </summary>
    /// <param name="neighbouredEdges">An array that reprents the edges that are on collision with other cells</param>
    public void SetTileEdges(bool[] neighbouredEdges)
    {
        edges = neighbouredEdges;
    }

    /// <summary>
    /// Plays the update or spawn animation. It is called from each PlantRenderer to animate cells when updating them. 
    /// </summary>
    public void TriggerAnimation()
    {
        if (!alreadySpawned) animator.Play("Spawn");
        else animator.Play("Update");
    }

    /// <summary>
    /// Updates the rendered sprite so that it matches stored edge data. (Called by Animator on precise frames). 
    /// </summary>
    public void UpdateSprite()
    {
        int[] indices = GetTileIndices();
        Vector4 indexData = new Vector4(indices[0], indices[1], indices[2], indices[3]);
        tileRenderer.material.SetVector("_TileIndex", indexData);
        alreadySpawned = true; //Will no longer play spawn animation, instead it will use Update animation
    }

    /// <summary>
    /// Plays a destroying animation and then deletes the instance of the cell renderer. 
    /// </summary>
    /// <param name="animate">Whether the instance will play or not its destroy animation before being deleted</param>
    public void DestroyCell(bool animate = true)
    {
        if (animate) animator.Play("Destroy"); //Delete will be called on animation end
        else Delete(); //Instant deletion
    }

    /// <summary>
    /// DO NOT USE. CALL "DestroyCell()" INSTEAD! (This needs to be public for animation purposes). 
    /// </summary>
    public void Delete()
    {
        Destroy(gameObject);
    }

    int GetSubTileIndex(bool[] corners)
    {
        //Gets the sprite index for a subtile (one of the four small tiles that form a normal one). 
        int index = 0;
        foreach (bool corner in corners){
            index += corner ? 1 : 0;
            index <<= 1;
        }

        return index;
    }

    int[] GetTileIndices() //Returns 4 indices (the sub-sprites)
    {
        int [] indices = new int[]{
            GetSubTileIndex(new bool[]{edges[0],edges[1],edges[3],edges[4]}),
            GetSubTileIndex(new bool[]{edges[1],edges[2],edges[4],edges[5]}),
            GetSubTileIndex(new bool[]{edges[3],edges[4],edges[6],edges[7]}),
            GetSubTileIndex(new bool[]{edges[4],edges[5],edges[7],edges[8]}),
        };

        return indices;
    }
}
