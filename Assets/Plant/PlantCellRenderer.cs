using System;
using System.Collections;
using Lvl3Mage.EditorEnhancements.Runtime;
using Lvl3Mage.InterpolationToolkit.Splines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class PlantCellRenderer : MonoBehaviour
{
    [SerializeField] Renderer tileRenderer;
    [SerializeField] Animator animator;
    int pastTileIndex = 0;
    [SerializeField] int tileIndex = 0;
    Color[] quadrantColors = new Color[4];
    bool[] testQuadrantData = new bool[4];
    [SerializeField] VibrationSplineCreator shakeSpline;
    [SerializeField] float shakeDuration = 1f;
    
    
    

    void OnDrawGizmos()
    {
        Vector2Int[] quadrants = CellUtils.CellQuadrantOffsets();
        for (int i = 0; i < quadrants.Length; i++){
            Gizmos.color = testQuadrantData[i] ? Color.red : Color.green;
            Gizmos.DrawWireCube(quadrants[i] - Vector2.one*0.5f + (Vector2)transform.position, Vector3.one*0.5f);
        }
    }

    /// <summary>Modifies the whole cell edges. Each PlantRenderer makes its childs call it. </summary>
    /// <param name="quadrantData">An array that reprents the edges that are on collision with other cells</param>
    /// <param name="newQuadrantColors"></param>
    public void SetData(bool[] quadrantData, Color[] newQuadrantColors)
    {
        testQuadrantData = quadrantData;
        pastTileIndex = tileIndex;
        tileIndex = GetTileIndex(quadrantData);
        
        quadrantColors = newQuadrantColors;
    }

    public void AnimateSpriteChange()
    {
        if (pastTileIndex == tileIndex){
            Debug.LogWarning("No change in tile index. Skipping animation", this);
            return;
        }
        if (pastTileIndex == 0){
            animator.Play("Spawn");
        }
        else if (tileIndex == 0){
            animator.Play("Destroy"); 
        }
        else{
            animator.Play("Update");
        }
    }
    /// <summary>Plays the "Shake" animation in the animator. </summary>
    public void AnimateShake()
    {
        StartCoroutine(Shake(shakeDuration));
        //animator.Play("Shake");
    }
    IEnumerator Shake(float duration)
    {
        Vector2 originalPosition = transform.position;
        ISpline xSpline = shakeSpline.CreateSpline(-1, 1);
        ISpline ySpline = shakeSpline.CreateSpline(-1, 1);
        float time = 0;
        while (time < duration){
            float t = time/duration;
            Vector2 position = originalPosition + new Vector2(xSpline.Evaluate(t), ySpline.Evaluate(t));
            transform.position = position;
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }

    /// <summary>Destroys the gameObjectInstance. </summary>
    public void Delete() => Destroy(gameObject);
    /// <summary>Updates the rendered sprite so that it matches stored edge data. (Called by Animator on precise frames). </summary>
    static int GetTileIndex(bool[] tileData)
    {
        int index = 0;
        for (int i = tileData.Length-1; i >= 0; i--){
            index <<= 1;
            index += tileData[i] ? 1 : 0;
        }

        return index;

    }
    public void UpdateSprite()
    {
        tileRenderer.material.SetFloat("_TileIndex", tileIndex);
        tileRenderer.material.SetColor("_TintBottomLeft", quadrantColors[0]);
        tileRenderer.material.SetColor("_TintBottomRight", quadrantColors[1]);
        tileRenderer.material.SetColor("_TintTopLeft", quadrantColors[2]);
        tileRenderer.material.SetColor("_TintTopRight", quadrantColors[3]);
        
    }
}
