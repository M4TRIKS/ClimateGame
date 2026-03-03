using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType   // tipes of tiles
{
    Grass,
    Water,
    Sand,
    Rock
}

public class Tile : MonoBehaviour
{
    ///Factory
    [SerializeField] private GameObject _buildingPrefab;
    private bool _isBuilt = false;
    private GameObject _currentBuilding;
    // The main color of the tile
    [SerializeField] private Color _grassColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Color _sandColor;
    [SerializeField] private Color _rockColor;

    // Reference to the SpriteRenderer component (to see the tile)
    [SerializeField] private SpriteRenderer _renderer;

    // Reference to a highlight GameObject
    [SerializeField] private GameObject _highlight;     // it will change depending if the mouse is on top

    private TileType _tileType; // Stores the tile type, SHOULD I MAKE TIS PUBLIc???

// CALLED by grid manager
    public void Init(TileType type)    
    {
        _tileType = type; // stores tile for later 

        switch (type) //checks the types of tile and gives a specific render
        {
            case TileType.Grass:
                _renderer.color = _grassColor;
                break;

            case TileType.Water:
                _renderer.color = _waterColor;
                break;

            case TileType.Sand:
                _renderer.color = _sandColor;
                break;

            case TileType.Rock:
                _renderer.color = _rockColor;
                break;
        }
    }

//it allows what kind of tile is but without being able to change it.
    public TileType GetTileType()
    {
        return _tileType;
    }

// highlight when the mouse is on the tile
    void OnMouseEnter()
    {

        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }


//////////// BUILDING
    public bool IsBuilt()
{
    return _isBuilt;
}
/// <summary>
///  builds only if the tile is free
/// </summary>
public bool CanBuild()
{
    if (_isBuilt) return false; //if is build or not
    if (_tileType == TileType.Water) return false; // not being able to build on water

    return true;
}
public void Build()
{
    
  /*   if (_isBuilt) return;
    if (_tileType == TileType.Water) return; // not being able to build on water
    */
        
        _currentBuilding = Instantiate(_buildingPrefab, transform.position, Quaternion.identity);
        _isBuilt = true;
        
        
} 
}
