using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _pickUpClip, _dropClip;

    [SerializeField] private FactoryData[] _factoryPool;
    [SerializeField] private SpriteRenderer _renderer;
    //draging animation
    [Header("Drag Visuals")]

    [SerializeField] private float _dragScaleMultiplier = 0.8f;
    [SerializeField] private float _dragAlpha = 0.6f;
    [Header("Drag Motion")]
    [SerializeField] private float _followSmoothness = 157.5f;
    [SerializeField] private float _maxTiltAngle = 130f;
    [SerializeField] private float _tiltStrength = 8.8f;
    [SerializeField] private float _rotationSmoothness = 17f;


    private Vector3 _originalScale;
    private Color _originalColor;
    private Quaternion _originalRotation;
    private Vector3 _dragVelocity;
    private Vector3 _lastMouseWorldPos;

    private Collider2D _collider;
    private bool _dragging;
    private Vector2 _offset, _originalPosition;

    private FactoryData _currentFactoryData;
    private readonly List<Tile> _previewTiles = new List<Tile>();

    void Awake()
    {
        _originalPosition = transform.position;
        _collider = GetComponent<Collider2D>();

        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();

        // saving the original values of the factorie
        _originalScale = transform.localScale;
        _originalRotation = transform.rotation;


        if (_renderer != null)
        _originalColor = _renderer.color;

        RollNextFactory();
    }

    void Update()
    {
        // stop dragging if game is paused or ended
        if (IsInputBlocked())
        {
            if (_dragging)
            {
                CancelDrag();
            }
            return;
        }

        if (!_dragging) return;

        Vector3 mousePosition = GetMousePos() - _offset;

        // smooth movement
        transform.position = Vector3.Lerp(
            transform.position,
            mousePosition,
            _followSmoothness * Time.deltaTime
        );

        // calculate mouse movement speed
        Vector3 currentMouseWorldPos = GetMousePos();
        Vector3 mouseDelta = currentMouseWorldPos - _lastMouseWorldPos;

        // tilt depending on horizontal movement
        float targetZRotation = Mathf.Clamp(
            -mouseDelta.x * _tiltStrength * 100f,
            -_maxTiltAngle,
            _maxTiltAngle
        );

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            _rotationSmoothness * Time.deltaTime
        );

        _lastMouseWorldPos = currentMouseWorldPos;

        UpdateComboPreview();
    }
    

    void OnMouseDown()
    {
        if (IsInputBlocked()) return;

        _dragging = true;
        ApplyDragVisuals(true);
        ClearComboPreview();

        if (_source != null && _pickUpClip != null)
            _source.PlayOneShot(_pickUpClip);

        _offset = GetMousePos() - (Vector2)transform.position;
        _lastMouseWorldPos = GetMousePos();


        // disable collider while dragging
        if (_collider != null)
            _collider.enabled = false;

    }

    void OnMouseUp()
    {
        if (IsInputBlocked())
        {
            CancelDrag();
            return;
        }

        SpawnFactories();
        ClearComboPreview();

        // return draggable object to original place
        transform.position = _originalPosition;
        _dragging = false;
        ApplyDragVisuals(false);
        

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
            // choose next factory only if build succeeded
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

        // ignore null entries in pool
        var validFactories = System.Array.FindAll(_factoryPool, f => f != null);

        if (validFactories.Length == 0)
        {
            Debug.LogError("Factory pool has no valid FactoryData!");
            _currentFactoryData = null;
            return;
        }

        // pick random factory
        _currentFactoryData = validFactories[Random.Range(0, validFactories.Length)];

        // show first level sprite in draggable preview
        if (_renderer != null &&
            _currentFactoryData.levels != null &&
            _currentFactoryData.levels.Length > 0 &&
            _currentFactoryData.levels[0].sprite != null)
        {
            _renderer.sprite = _currentFactoryData.levels[0].sprite;
        }
        if (_renderer != null)
        {
            _renderer.color = new Color(1f, 1f, 1f, 1f);
            _originalColor = _renderer.color;
        }
    }

    void UpdateComboPreview()
    {
        ClearComboPreview();

        if (_currentFactoryData == null) return;
        if (_currentFactoryData.comboPattern == null || _currentFactoryData.comboPattern.Length == 0) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mouseWorldPos.x);
        int y = Mathf.RoundToInt(mouseWorldPos.y);

        Tile centerTile = _gridManager.GetTileAtPosition(new Vector2Int(x, y));

        if (centerTile == null) return;

        // show preview on all pattern tiles
        foreach (Vector2Int offset in _currentFactoryData.comboPattern)
        {
            Tile targetTile = _gridManager.GetTileAtPosition(centerTile.GetGridPosition() + offset);

            if (targetTile != null)
            {
                targetTile.ShowComboPreview();
                _previewTiles.Add(targetTile);
            }
        }
    }

    void ClearComboPreview()
    {
        for (int i = 0; i < _previewTiles.Count; i++)
        {
            if (_previewTiles[i] != null)
                _previewTiles[i].HideComboPreview();
        }

        _previewTiles.Clear();
    }

    /// <summary>
    /// /problem of keep draging while paused
    /// </summary>
    bool IsInputBlocked()
    {
        if (_gameManager != null && _gameManager.IsGameEnded)
            return true;

        if (Time.timeScale == 0f)
            return true;

        return false;
    }

    void CancelDrag()
    {
        ClearComboPreview();
        transform.position = _originalPosition;
        _dragging = false;
        ApplyDragVisuals(false);

        if (_collider != null)
            _collider.enabled = true;
    }

    void ApplyDragVisuals(bool isDragging)
{
    if (_renderer == null) return;

    if (isDragging)
    {
        transform.localScale = _originalScale * _dragScaleMultiplier;

        Color color = _renderer.color;
        color.a = _dragAlpha;
        _renderer.color = color;
    }
    else
    {
        transform.localScale = _originalScale;
        _renderer.color = _originalColor;
        transform.rotation = _originalRotation;

    }
}

}