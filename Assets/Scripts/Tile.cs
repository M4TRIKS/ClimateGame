using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType   // tipes of tiles
{
    Grass,
    Water,
    Sand,
    Rock,

    Pollution
}

public class Tile : MonoBehaviour
{
    [Header("visuals")]

    [SerializeField] private GameObject _comboPreviewHighlight;
    ///Factory
    [SerializeField] private GameObject _buildingPrefab;
    private bool _isBuilt = false;
    private GameObject _currentBuilding;
    // The main color of the tile
    [SerializeField] private Color _grassColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Color _sandColor;
    [SerializeField] private Color _rockColor;
    [SerializeField] private Color _pollutionColor;

    // Reference to the SpriteRenderer component (to see the tile)
    [SerializeField] private SpriteRenderer _renderer;

    // Reference to a highlight GameObject
    [SerializeField] private GameObject _highlight;     // it will change depending if the mouse is on top
    private Vector2Int _gridPosition; // /////////////////Remembers its X,Y 


    public Factory CurrentFactory { get; private set; } // Lets other scripts access the factory on this tile
    private TileType _tileType; // Stores the tile type, SHOULD I MAKE TIS PUBLIc???

    // Pollution system
    private float _pollutionChance = 0f;






// CALLED by grid manager
public void ConvertToPollution()
{
    if (_tileType == TileType.Water) return; // not able to pollut water

    _tileType = TileType.Pollution;
    _renderer.color = _pollutionColor;
  // Tell the existing factory it has been polluted
 
      
    if (_isBuilt && CurrentFactory != null)
        {
    // Recalculate tile bonus if tile becomes polluted
        CurrentFactory.Init(GetTileBonus());
        }
       // CurrentFactory.Init(GetTileBonus()); // Since the bonus saves at the begining once the factory is build I need to update it
    

    Debug.Log("Tile has been polluted");


}

    public void Init(TileType type, Vector2Int pos)    
    {
        _tileType = type; // stores tile for later 
        _gridPosition = pos; //position

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

            case TileType.Pollution:
            _renderer.color = _pollutionColor;

                break;
        }
    if (_highlight != null)
        _highlight.SetActive(false);

    if (_comboPreviewHighlight != null)
        _comboPreviewHighlight.SetActive(false);
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
 
  /*   if (_isBuilt) return;
    if (_tileType == TileType.Water) return; // not being able to build on water
    */
     /*    
        _currentBuilding = Instantiate(_buildingPrefab, transform.position, Quaternion.identity);
        _isBuilt = true;
        
         */


public void Build(FactoryData data)
{
        // Spawn factory prefab (as a child of the tile)

    _currentBuilding = Instantiate(_buildingPrefab, transform.position, Quaternion.identity, transform);    
    // Save the reference to CurrentFactory so the ComboManager can upgrade it later!
    CurrentFactory = _currentBuilding.GetComponent<Factory>();

    if (CurrentFactory != null)
    {   
        // Give the factory its ScriptableObject data

        CurrentFactory.SetData(data);     
        // Initialize the factory with the tile bonus

        CurrentFactory.Init(GetTileBonus());
    }
    _isBuilt = true;
}

public int GetTileBonus()
{
    switch (_tileType)
    {
        case TileType.Sand:
            return 1;

        case TileType.Rock:
            return 2;   

        case TileType.Grass:
            return 3;

        case TileType.Pollution:
        return 0;

        default:
            return 0;
    }
}
public Vector2Int GetGridPosition()
{
    return _gridPosition;
}
 public void SetGridPosition(Vector2Int pos)
    {
        _gridPosition = pos;
    }

///
/////////////////////////////POLLUTION
/// ///
    public void AddPollutionChance(float amount)
{
    if (_tileType == TileType.Water) return;
    if (_tileType == TileType.Pollution) return;

    _pollutionChance += amount;

    // clamp so it never exceeds 100%
    _pollutionChance = Mathf.Clamp(_pollutionChance, 0f, 1f);

    TryPollute();
}
private void TryPollute()
/////////EVERYTIME IT GETS UPGRADED OR THERE IS A NEW FACTORY IT COULD BE POLLUTED 
{
    float roll = Random.value; // random number between 0 and 1

    if (roll <= _pollutionChance)
    {
        ConvertToPollution();
    }
}


/// combo preview
public void ShowComboPreview()
{
    if (_comboPreviewHighlight != null)
        _comboPreviewHighlight.SetActive(true);
}

public void HideComboPreview()
{
    if (_comboPreviewHighlight != null)
        _comboPreviewHighlight.SetActive(false);
}
}
