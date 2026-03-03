using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private TextMeshProUGUI _resourceText;

    [SerializeField] private int _factoryCost = 25;
    private int _resources = 15;
    private float _resourceTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 



// Update is called once per frame
    void Update()
    {
        
       //  float EnergyPuntuation = load TextMesh pro <Text>
       //Will create resources every second
        _resourceTimer += Time.deltaTime;

        if (_resourceTimer >= 1.2f)
        {
            GenerateResources();
            _resourceTimer = 0f;
        }

         //Find the tileclicked on


            
        // create a fatory when left clink
        //new tiles  position Vector3(10.5,8.81000042,0) to drag
        //////////// test of construction /////////
        /* if (Input.GetMouseButtonDown(0))
        {
            Tile clickedTile = GetTileAtMousePosition();

            if (clickedTile == null) return;
            //using methods of tiles script
            if (!clickedTile.IsBuilt())
            {
                clickedTile.Build();
            }
        } */



     

            //Make a decision based on the answer
          /*   if (type == TileType.Water)
            {
               // add 0
            }
            else
            {
            } */
        

    }
    /// getting position of the tiles
Tile GetTileAtMousePosition()
{
    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    int x = Mathf.RoundToInt(mouseWorldPos.x);
    int y = Mathf.RoundToInt(mouseWorldPos.y);

    return _gridManager.GetTileAtPosition(new Vector2(x, y));
}

// calculating fuction of resources
void GenerateResources()
{
    foreach (var tile in _gridManager.GetAllTiles())
    {
        //if there is a building will call the fuction of which tipe of building to get the multiplier
        if (tile.IsBuilt())
        {
            int multiplier = GetMultiplier(tile.GetTileType());
            _resources += 1 * multiplier;
        }
    }

    _resourceText.text = "Resources: " + _resources;
}
int GetMultiplier(TileType type) // an other swtich to check the type of tile
{
    switch (type)
    {
        case TileType.Water:
            return 1; //now is nnot able to do it in water

        case TileType.Sand:
            return 1;

        case TileType.Rock:
            return 2;

        case TileType.Grass:
            return 3;

        default:
            return 1;
    }
}
public bool TryBuild(Tile tile)
{
    if (!tile.CanBuild()) return false;

    if (_resources < _factoryCost)
    {
        Debug.Log("Not enough resources");
        return false;
    }

    tile.Build();
    _resources -= _factoryCost;
    UpdateUI();

    return true;
}
void UpdateUI()
{
    _resourceText.text = "Resources: " + _resources;
}



}


