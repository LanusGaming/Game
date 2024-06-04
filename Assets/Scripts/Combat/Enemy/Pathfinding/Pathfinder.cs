using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Pathfinder
{
    public Tilemap floor;
    public int subDivisions = 2;
    
    private bool[,] walkableSurface;

    public void GenerateWalkableMesh()
    {
        foreach (var position in floor.cellBounds.allPositionsWithin)
        {
            Debug.Log($"{position}: {floor.GetTile(position).name}");
        }
    }
}
