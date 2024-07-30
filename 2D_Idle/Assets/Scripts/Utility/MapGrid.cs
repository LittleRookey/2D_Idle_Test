using Litkey.Weather;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f; // Size of each cell in the grid
    public bool isXZ; // 3D¿ë
    private int gridSize;

    [SerializeField] private Tile[,] mapTiles;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
} 

public class Tile
{
    public int index;
    public Sprite tileIcon;
    public eTileType tileType;
    [InlineEditor]
    public Health Monster;
    public Health Boss;
    [Range(0.1f, 100f)]
    public float difficultyRate = 1f;
    
    public Weather weather;
    public eResourceType appearingResourceType;
    

}
public enum eTileType
{
    Home,
    Battle,
    Boss,
    Shop,
    SecretShop,
}
