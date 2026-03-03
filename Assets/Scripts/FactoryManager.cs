using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class FactoryManager : MonoBehaviour
{ 
[SerializeField] private GameManager _gameManager;

[SerializeField] private GridManager _gridManager;
[SerializeField] private AudioSource _source;
[SerializeField] private AudioClip _pickUpClip, _dropClip;
private Collider2D _collider;

private bool _dragging;
    // _offset: Distance between the mouse and the center of the object (prevents snapping)
    // _originalPosition: Where the "source" factory stays
private Vector2 _offset, _originalPosition;
    void Awake()
    {
        // Store the starting position so the "Ghost" factory can return to the menu
        _originalPosition = transform.position;
        _collider = GetComponent<Collider2D>();

    }


    void Update()
    {

        if(!_dragging) return;   
        var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);  // Follow the mouse: Update the position every frame

        transform.position  = mousePosition - _offset; 

    } 
    void OnMouseDown()
    {
        _dragging = true;
        _source.PlayOneShot(_pickUpClip);
        // Calculate the gap between the mouse and the object's center 
        // to keep the drag looking smooth and natural.

        _offset = GetMousePos() - (Vector2)transform.position;
       _collider.enabled = false; // unable collider while I drag (It will let the tile higlight work)

    }
    void OnMouseUp()
    {
        SpawnFactories(); // Attempt to place a factory at the current mouse spot
    
        transform.position  = _originalPosition; // Return  back to its starting spot in the sidebar
        _dragging = false;
       _collider.enabled = true; // enable
        _source.PlayOneShot(_dropClip);
    }
    
        Vector2 GetMousePos() // Helper function to convert mouse pixels to world coordinates
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }
void SpawnFactories()
{
    //Where is the mouse right now?
    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //Round the position to find the nearest Grid coordinate

    int x = Mathf.RoundToInt(mouseWorldPos.x);
    int y = Mathf.RoundToInt(mouseWorldPos.y);
    //asks grid manager if there is a tile in the same coordenates

    Tile tile = _gridManager.GetTileAtPosition(new Vector2(x, y));
    
    //If we found a tile, tell the GameManager to try and build there

    if (tile == null) return;
    
    _gameManager.TryBuild(tile); 

}
}

