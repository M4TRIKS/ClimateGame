using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType   // types of tiles
{
    Grass,
    Water,
    Sand,
    Rock,
    Pollution,
    Fire
}

public class Tile : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject _comboPreviewHighlight;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight; // highlight when mouse is on tile

    [Header("Tile Sprites")]
    [SerializeField] private Sprite[] _grassSprites;
    [SerializeField] private Sprite[] _sandSprites;
    [SerializeField] private Sprite[] _rockSprites;
    [SerializeField] private Sprite[] _pollutionSprites;
    
    [Header("Fire Animation")]
    [SerializeField] private Sprite[] _fireAnimationFrames;
    [SerializeField] private float _fireFrameRate = 0.6f;
    [Header("Water Animation")]
    [SerializeField] private Sprite[] _waterAnimationFrames;
    [SerializeField] private float _waterFrameRate = 0.5f;

    [Header("Fallback Colors")]
    [SerializeField] private Color _grassColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Color _sandColor;
    [SerializeField] private Color _rockColor;
    [SerializeField] private Color _pollutionColor;
    [SerializeField] private Color _fireColor;

    [Header("Factory")]
    [SerializeField] private GameObject _buildingPrefab;

    private bool _isBuilt = false;
    private GameObject _currentBuilding;
    private Vector2Int _gridPosition;

    public Factory CurrentFactory { get; private set; }
    private TileType _tileType;

    // Pollution system
    private float _pollutionChance = 0f;

    // Water animation coroutine
    private Coroutine _tileAnimationCoroutine;
    // CALLED by grid manager
    public void ConvertToPollution()
    {
        if (_tileType == TileType.Water) return; // cannot pollute water

        _tileType = TileType.Pollution;
        UpdateTileVisual();

        // Recalculate tile bonus if tile becomes polluted
        if (_isBuilt && CurrentFactory != null)
        {
            CurrentFactory.Init(GetTileBonus());
        }

        Debug.Log("Tile has been polluted");
    }

    public void Init(TileType type, Vector2Int pos)
    {
        _tileType = type;
        _gridPosition = pos;

        UpdateTileVisual();

        if (_highlight != null)
            _highlight.SetActive(false);

        if (_comboPreviewHighlight != null)
            _comboPreviewHighlight.SetActive(false);
    }

    // lets other scripts check the tile type without changing it
    public TileType GetTileType()
    {
        return _tileType;
    }

    // highlight when the mouse is on the tile
    void OnMouseEnter()
    {
        if (_highlight != null)
            _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        if (_highlight != null)
            _highlight.SetActive(false);
    }

    ////////////// BUILDING
    public bool IsBuilt()
    {
        return _isBuilt;
    }

    /// <summary>
    /// builds only if the tile is free
    /// </summary>
    public bool CanBuild()
    {
        if (_isBuilt) return false;
        if (_tileType == TileType.Water) return false;
        if (_tileType == TileType.Fire) return false;

        return true;
    }

    public void Build(FactoryData data)
    {
        // Spawn factory prefab as a child of the tile
        _currentBuilding = Instantiate(_buildingPrefab, transform.position, Quaternion.identity, transform);

        // Save the reference so ComboManager can access it later
        CurrentFactory = _currentBuilding.GetComponent<Factory>();

        if (CurrentFactory != null)
        {
            CurrentFactory.SetData(data);
            CurrentFactory.Init(GetTileBonus());
        }

        _isBuilt = true;
    }

    public int GetTileBonus()
    {
        switch (_tileType)
        {
            case TileType.Sand:
                return 2;

            case TileType.Rock:
                return 1;

            case TileType.Grass:
                return 3;

            case TileType.Pollution:
                return 0;

            case TileType.Fire:
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

    ///////////////////////////// POLLUTION
    public void AddPollutionChance(float amount)
    {
        if (_tileType == TileType.Water) return;
        if (_tileType == TileType.Pollution) return;

        _pollutionChance += amount;

        // Clamp so it never exceeds 100%
        _pollutionChance = Mathf.Clamp(_pollutionChance, 0f, 1f);

        TryPollute();
    }

    private void TryPollute()
    {
        // Every time pollution is added, the tile has a chance to become polluted
        float roll = Random.value;

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

    /// <summary>
    /// FIRE EVENT METHODS
    /// </summary>
    public void ConvertToFire()
    {
        if (_tileType == TileType.Water) return;
        if (_tileType == TileType.Fire) return;

        _tileType = TileType.Fire;
        UpdateTileVisual();

        if (_isBuilt)
        {
            DestroyBuilding();
        }

        Debug.Log($"Tile {name} is now on fire!");
    }

    public void DestroyBuilding()
    {
        if (_currentBuilding != null)
        {
            Destroy(_currentBuilding);
        }

        _currentBuilding = null;
        CurrentFactory = null;
        _isBuilt = false;
    }

    ////// water event
    public void ConvertToWater()
    {
        if (_tileType == TileType.Water) return;

        _tileType = TileType.Water;
        UpdateTileVisual();

        if (_isBuilt)
        {
            DestroyBuilding();
        }

        Debug.Log($"Tile {name} has been flooded!");
    }

    // =========================
    // VISUALS
    // =========================

   private void UpdateTileVisual()
    {
        StopTileAnimation();

        switch (_tileType)
        {
            case TileType.Grass:
                if (!SetRandomSprite(_grassSprites))
                    SetColorOnly(_grassColor);
                break;

            case TileType.Water:
                if (_waterAnimationFrames != null && _waterAnimationFrames.Length > 0)
                    StartTileAnimation(_waterAnimationFrames, _waterFrameRate);
                else
                    SetColorOnly(_waterColor);
                break;

            case TileType.Sand:
                if (!SetRandomSprite(_sandSprites))
                    SetColorOnly(_sandColor);
                break;

            case TileType.Rock:
                if (!SetRandomSprite(_rockSprites))
                    SetColorOnly(_rockColor);
                break;

            case TileType.Pollution:
                if (!SetRandomSprite(_pollutionSprites))
                    SetColorOnly(_pollutionColor);
                break;

            case TileType.Fire:
                if (_fireAnimationFrames != null && _fireAnimationFrames.Length > 0)
                    StartTileAnimation(_fireAnimationFrames, _fireFrameRate);
                else
                    SetColorOnly(_fireColor);
                break;
        }
    }

    private bool SetRandomSprite(Sprite[] spriteArray)
        {
            if (_renderer == null) return false;
            if (spriteArray == null || spriteArray.Length == 0) return false;

            int randomIndex = Random.Range(0, spriteArray.Length);
            _renderer.sprite = spriteArray[randomIndex];
            _renderer.color = Color.white;
            return true;
        }

    private void SetColorOnly(Color color)
        {
            if (_renderer == null) return;

            _renderer.sprite = null;
            _renderer.color = color;
        }

    private void StartTileAnimation(Sprite[] frames, float frameRate)
        {
            if (_renderer == null) return;
            if (frames == null || frames.Length == 0) return;

            _tileAnimationCoroutine = StartCoroutine(AnimateTile(frames, frameRate));
        }

    private void StopTileAnimation()
        {
            if (_tileAnimationCoroutine != null)
            {
                StopCoroutine(_tileAnimationCoroutine);
                _tileAnimationCoroutine = null;
            }
        }

    private IEnumerator AnimateTile(Sprite[] frames, float frameRate)
    {
        int frameIndex = Random.Range(0, frames.Length);

        while (true)
        {
            _renderer.sprite = frames[frameIndex];
            _renderer.color = Color.white;

            frameIndex = (frameIndex + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}