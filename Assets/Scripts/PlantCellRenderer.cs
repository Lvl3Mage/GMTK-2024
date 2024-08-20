#nullable enable
using System.Collections;
using Lvl3Mage.EditorEnhancements.Runtime;
using UnityEngine;

public class PlantCellRenderer : MonoBehaviour
{
    [SerializeField] Renderer tileRenderer;
    [SerializeField] Animator animator;
    int pastTileIndex = 0;
    [SerializeField] int tileIndex = 0;
    Color[] quadrantColors = new Color[4];
    [SerializeField] float shakeDuration = 1f;

    [SerializeField] ParticleSystem particles;

    [SerializeField] [ParentActionButton("Apply Changes", nameof(UpdateSprite), hideField:true)]
    string btn;

    void Awake()
    {
        tileRenderer.material.SetFloat("_TileIndex", 0);
        particles = GetComponent<ParticleSystem>();
    }

    public bool IsRendering()
    {
        return tileIndex != 0;
    }

    /// <summary>Modifies the whole cell edges. Each PlantRenderer makes its childs call it. </summary>
    /// <param name="quadrantData">An array that reprents the edges that are on collision with other cells</param>
    /// <param name="newQuadrantColors"></param>
    public void SetData(bool[] quadrantData, Color[] newQuadrantColors)
    {
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
    }
    IEnumerator Shake(float duration)
    {
        tileRenderer.material.SetFloat("_NoiseMultiplier", 22);
        yield return new WaitForSeconds(duration);
        tileRenderer.material.SetFloat("_NoiseMultiplier", 1);
        // Vector2 originalPosition = transform.position;
        // ISpline xSpline = shakeSpline.CreateSpline(-1, 1);
        // ISpline ySpline = shakeSpline.CreateSpline(-1, 1);
        // float time = 0;
        // while (time < duration){
        //     float t = time/duration;
        //     Vector2 position = originalPosition + new Vector2(xSpline.Evaluate(t), ySpline.Evaluate(t));
        //     transform.position = position;
        //     time += Time.deltaTime;
        //     yield return null;
        // }
        // transform.position = originalPosition;
    }

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
        if (tileIndex == 0){
            tileRenderer.enabled = false;
            return;
        }

        tileRenderer.enabled = true;
        tileRenderer.material.SetFloat("_TileIndex", tileIndex);
        tileRenderer.material.SetColor("_TintBottomLeft", quadrantColors[0]);
        tileRenderer.material.SetColor("_TintBottomRight", quadrantColors[1]);
        tileRenderer.material.SetColor("_TintTopLeft", quadrantColors[2]);
        tileRenderer.material.SetColor("_TintTopRight", quadrantColors[3]);
        
        
    }
}
