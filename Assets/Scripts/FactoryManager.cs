using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _pickUpClip, _dropClip;

    [SerializeField] private FactoryData[] _factoryPool;
    [SerializeField] private SpriteRenderer _renderer;

    private Collider2D _collider;
    private bool _dragging;
    private Vector2 _offset, _originalPosition;

    private FactoryData _currentFactoryData;

    void Awake()
    {
        _originalPosition = transform.position;
        _collider = GetComponent<Collider2D>();

        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();

        RollNextFactory();
    }

    void Update()
    {
        if (!_dragging) return;

        var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition - _offset;
    }

    void OnMouseDown()
    {
        _dragging = true;

        if (_source != null && _pickUpClip != null)
            _source.PlayOneShot(_pickUpClip);

        _offset = GetMousePos() - (Vector2)transform.position;

        if (_collider != null)
            _collider.enabled = false;
    }

    void OnMouseUp()
    {
        SpawnFactories();

        transform.position = _originalPosition;
        _dragging = false;

        if (_collider != null)
            _collider.enabled = true;

        if (_source != null && _dropClip != null)
            _source.PlayOneShot(_dropClip);
    }

    Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void SpawnFactories()
    {
        if (_currentFactoryData == null)
        {
            Debug.LogError("No current factory selected!");
            return;
        }

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int x = Mathf.RoundToInt(mouseWorldPos.x);
        int y = Mathf.RoundToInt(mouseWorldPos.y);

        Tile tile = _gridManager.GetTileAtPosition(new Vector2Int(x, y));

        if (tile == null) return;

        bool success = _gameManager.TryBuild(tile, _currentFactoryData);

        if (success)
        {
            RollNextFactory();
        }
        else
        {
            Debug.Log("Could not build here");
        }
    }

    void RollNextFactory()
    {
        if (_factoryPool == null || _factoryPool.Length == 0)
        {
            Debug.LogError("Factory pool is empty!");
            _currentFactoryData = null;
            return;
        }

        var validFactories = System.Array.FindAll(_factoryPool, f => f != null);

        if (validFactories.Length == 0)
        {
            Debug.LogError("Factory pool has no valid FactoryData!");
            _currentFactoryData = null;
            return;
        }

        _currentFactoryData = validFactories[Random.Range(0, validFactories.Length)];

        if (_renderer != null && _currentFactoryData.sprite != null)
        {
            _renderer.sprite = _currentFactoryData.sprite;
        }
    }
}