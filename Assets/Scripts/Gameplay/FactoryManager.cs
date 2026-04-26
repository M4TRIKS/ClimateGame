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
    [SerializeField] private SpriteRenderer _hoverGlowRenderer;
    [SerializeField] private GameObject _hoverGlowObject;
    //draging animation
    [Header("Drag Visuals")]

    [SerializeField] private float _dragScaleMultiplier = 0.8f;
    [SerializeField] private float _dragAlpha = 0.6f;
    [Header("Drag Motion")]
    [SerializeField] private float _followSmoothness = 157.5f;
    [SerializeField] private float _maxTiltAngle = 130f;
    [SerializeField] private float _tiltStrength = 8.8f;
    [SerializeField] private float _rotationSmoothness = 17f;

    public static bool IsDraggingFactory { get; private set; }
    private bool _showingFullPreviewTooltip = false;
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

    private Tile _currentHoverTile;

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
        UpdateTileHoverWhileDragging();
    }
    

    void OnMouseDown()
    {
        if (IsInputBlocked()) return;

   /*   It feels better if the player is able to drag it even with not enough money
      if (_gameManager != null && !_gameManager.CanAffordFactory())
        {
            TooltipUI.Hide_Static();
            TooltipWarningUI.Show_Static(
                $"Not enough money! Need {_gameManager.GetFactoryCost()}$",
                TooltipWarningUI.WarningType.NotEnoughMoney,
                1.2f
            );
            return;
        } */
        //highlight
        if (_hoverGlowObject != null)
        _hoverGlowObject.SetActive(false);

        _dragging = true;
        IsDraggingFactory = true;
        ApplyDragVisuals(true);
        TooltipUI.Hide_Static();
        ClearComboPreview();

        if (_source != null && _pickUpClip != null)
            _source.PlayOneShot(_pickUpClip);

        _offset = GetMousePos() - (Vector2)transform.position;
        _lastMouseWorldPos = GetMousePos();

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
        ClearTileHoverWhileDragging();

        // return draggable object to original place
        transform.position = _originalPosition;
        _dragging = false;
        IsDraggingFactory = false;
        TooltipUI.Hide_Static();
        ApplyDragVisuals(false);
        

        if (_collider != null)
            _collider.enabled = true;

        if (_source != null && _dropClip != null)
            _source.PlayOneShot(_dropClip, 4f);
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

            //highlight will show
            if (_hoverGlowRenderer != null)
            {
                _hoverGlowRenderer.sprite = _renderer.sprite;
            }
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
        ClearTileHoverWhileDragging();
        transform.position = _originalPosition;
        _dragging = false;
        IsDraggingFactory = false;
        TooltipUI.Hide_Static();
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
/// <summary>
/// ///////////// To show the data before building
/// </summary>
/// <returns></returns>
    void OnMouseEnter()
    {
        if (_dragging) return;
        if (TooltipWarningUI.IsShowing) return;
        if (_hoverGlowObject != null)
        _hoverGlowObject.SetActive(true);

        _showingFullPreviewTooltip = false;
        ShowShortPreviewTooltip();
    }

    void OnMouseExit()
    {
        _showingFullPreviewTooltip = false;
        TooltipUI.Hide_Static();
        if (_hoverGlowObject != null)
        _hoverGlowObject.SetActive(false);
    }

    void OnMouseOver()
    {
        if (_dragging) return;
        if (TooltipWarningUI.IsShowing) return;

        if (Input.GetMouseButtonDown(1))
        {
            _showingFullPreviewTooltip = true;
            TooltipUI.Show_Static(GetCurrentFactoryTooltipData());
        }
}
    TooltipData GetCurrentFactoryTooltipData()
    {
        if (_currentFactoryData == null)
            return new TooltipData("Factory", "No information available");

        FactoryLevelData levelData = null;

        if (_currentFactoryData.levels != null && _currentFactoryData.levels.Length > 0)
            levelData = _currentFactoryData.levels[0];

        string description = string.IsNullOrEmpty(_currentFactoryData.factoryDescription)
            ? "No description"
            : _currentFactoryData.factoryDescription;

        string comboPatternText = GetComboPatternText(_currentFactoryData.comboPattern);

        string body = description;

        if (levelData != null)
        {
            body += "\n\n" +
                    $"<b>Level:</b> 1\n" +
                    $"<b>Base Production:</b> {levelData.baseProduction}\n" +
                    $"<b>Cooldown:</b> {levelData.cooldown:0.##}s\n" +
                    $"<b>Combo Multiplier:</b> x{levelData.comboMultiplier:0.##}\n" +
                    $"<b>Combo Pattern:</b> {comboPatternText}";
        }
        else
        {
            body += "\n\n" +
                    $"<b>Combo Pattern:</b> {comboPatternText}";
        }

        Sprite icon = _currentFactoryData.tooltipSprite;

        if (icon == null &&
            _currentFactoryData.levels != null &&
            _currentFactoryData.levels.Length > 0 &&
            _currentFactoryData.levels[0].sprite != null)
        {
            icon = _currentFactoryData.levels[0].sprite;
        }

        string title = string.IsNullOrEmpty(_currentFactoryData.factoryName)
            ? "Factory"
            : _currentFactoryData.factoryName;

        return new TooltipData(title, body, icon);
    }
    string GetComboPatternText(Vector2Int[] pattern)
{
    if (pattern == null || pattern.Length == 0)
        return "None";

    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    for (int i = 0; i < pattern.Length; i++)
    {
        sb.Append($"({pattern[i].x}, {pattern[i].y})");

        if (i < pattern.Length - 1)
            sb.Append(", ");
    }

    return sb.ToString();
}
void ShowShortPreviewTooltip()
{
    if (_currentFactoryData == null)
    {
        TooltipUI.Show_Static(new TooltipData("Factory", "Right click for more info"));
        return;
    }

    string title = string.IsNullOrEmpty(_currentFactoryData.factoryName)
        ? "Factory"
        : _currentFactoryData.factoryName;

    TooltipUI.Show_Static(new TooltipData(
        title,
        "<i>Right click info</i>"
    ));
}


//fixing the  highlight so it workings directly from factory

void UpdateTileHoverWhileDragging()
{
    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    int x = Mathf.RoundToInt(mouseWorldPos.x);
    int y = Mathf.RoundToInt(mouseWorldPos.y);

    Tile tileUnderMouse = _gridManager.GetTileAtPosition(new Vector2Int(x, y));

    if (tileUnderMouse == _currentHoverTile)
        return;

    if (_currentHoverTile != null)
        _currentHoverTile.SetHoverHighlight(false);

    _currentHoverTile = tileUnderMouse;

    if (_currentHoverTile != null)
        _currentHoverTile.SetHoverHighlight(true);
}

void ClearTileHoverWhileDragging()
{
    if (_currentHoverTile != null)
    {
        _currentHoverTile.SetHoverHighlight(false);
        _currentHoverTile = null;
    }
}


}