using UnityEngine;

using System.Collections;    
using System.Collections.Generic;  

// This class is responsible for generating and managing the grid of tiles
public class GridManager : MonoBehaviour
{
    
    
    [SerializeField] private int _width, _height;    // Width and height of the grid (set in the Inspector)


    [SerializeField] private Tile _tilePrefab;     // Reference to the Tile prefab that will be spawned


    [SerializeField] private Transform _cam;     // Reference to the camera transform so we can center it
    // Dictionary to store all spawned tiles
    // Key = position (x,y)
    // Value = Tile object at that position
    private Dictionary<Vector2, Tile> _tiles;
    void Start()
    {
        GenerateGrid(); 
    }

    // Creates the full grid of tiles
    void GenerateGrid()
    {
        // Initialize the dictionary before using it
        _tiles = new Dictionary<Vector2, Tile>();

        // Loop of tiles
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Instantiate (create) a new tile at position (x, y, 0)
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity);

                // Rename the tile in the Hierarchy 
                spawnedTile.name = $"Tile {x} {y}";
           
                // Generate random TileType (Grass, Water, Sand, Rock)
                TileType randomType = (TileType)Random.Range(0, 4);

                // Initialize tile with that type
                spawnedTile.Init(randomType);

                // This allows us to access tiles later by their grid position
                _tiles[new Vector2(x, y)] = spawnedTile;   // Store the tile in the dictionary

            }
        }

        // Move the camera to the center of the grid
        _cam.transform.position = new Vector3(
            (float)_width / 2 - 0.5f,
            (float)_height / 2 - 0.1f, -10
        );
    }

    // Returns the tile at a specific grid position 
    //easy way to acces this in to the dicctionary
    public Tile GetTileAtPosition(Vector2 pos)  //<-----I need to call this inside update
    {
        // TryGetValue safely checks if the key exists
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile; // Return the tile if found
        }

        // If no tile exists at that position, return null
        return null;
    }


    public IEnumerable<Tile> GetAllTiles() //only reads
{
    return _tiles.Values; //returns the dictionary values
}
}
