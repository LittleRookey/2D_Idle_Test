using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f; // Size of each cell in the grid
    public bool isXZ; // 3D¿ë
    private int gridSize;

    [SerializeField] private Tile[,] mapTiles;
    

} 
public enum eTileType
{
    Defense,
    Boss,
}
