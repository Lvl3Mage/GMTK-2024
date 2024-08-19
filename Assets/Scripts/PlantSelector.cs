using System;
using System.Collections;
using UnityEngine;

class PlantSelector : MonoBehaviour
{
    [SerializeField] Vector2Int[] plantGeneratorOffsets;
    public IEnumerator SelectPlant()
    {
        while (false)
        {
            yield return null;
        }
    }

    public PlantGenerator GetPlantGenerator()
    {
        return position => {
            Vector2Int[] plantPositions = new Vector2Int[plantGeneratorOffsets.Length];
            for (int i = 0; i < plantGeneratorOffsets.Length; i++){
                plantPositions[i] = position + plantGeneratorOffsets[i];
            }

            return plantPositions;
        };
    }
}

public delegate Vector2Int[] PlantGenerator(Vector2Int rootPosition);