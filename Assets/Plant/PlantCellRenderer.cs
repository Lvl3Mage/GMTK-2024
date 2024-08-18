using UnityEngine;

public class PlantCellRenderer : MonoBehaviour
{
    [SerializeField] Renderer tileRenderer;
    [SerializeField] Animator animator;
    bool[] currentVertices = new bool[4];
    Color[] currentColors = new Color[4];

    /// <summary>Modifies the whole cell edges. Each PlantRenderer makes its childs call it. </summary>
    /// <param name="neighbouredVertices">An array that reprents the edges that are on collision with other cells</param>
    /// <param name="vertexColors"></param>
    public void SetData(bool[] neighbouredVertices, Color[] vertexColors)
    {
        currentColors = vertexColors;
        currentVertices = neighbouredVertices;
    }

    public void PlaySpawnAnimation() => animator.Play("Spawn");
    
    public void PlayUpdateAnimation() => animator.Play("Update");
    
    /// <summary>Plays a destroying animation and then deletes the instance of the cell renderer. </summary>
    public void PlayDestroyAnimation() => animator.Play("Destroy"); //Delete will be called on animation end

    /// <summary>Plays the "Shake" animation in the animator. </summary>
    public void PlayShakeAnimation() => animator.Play("Shake");

    /// <summary>Destroys the gameObjectInstance. </summary>
    public void Delete() => Destroy(gameObject);
    /// <summary>Updates the rendered sprite so that it matches stored edge data. (Called by Animator on precise frames). </summary>
    int GetTileIndex(bool[] tileData)
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
        int index = GetTileIndex(currentVertices);
        tileRenderer.material.SetFloat("_TileIndex", index);
        tileRenderer.material.SetColor("_TintBottomLeft", currentColors[0]);
        tileRenderer.material.SetColor("_TintBottomRight", currentColors[1]);
        tileRenderer.material.SetColor("_TintTopLeft", currentColors[2]);
        tileRenderer.material.SetColor("_TintTopRight", currentColors[3]);
        
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
