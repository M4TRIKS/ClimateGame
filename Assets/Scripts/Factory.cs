using UnityEngine;

public class Factory : MonoBehaviour
{
    // ScriptableObject that contains all the data for this factory type
    // (production, cooldown, combo pattern, sprite)
    [SerializeField] private FactoryData _data;

    // Bonus given by the tile (Grass, Rock, Sand etc)
    private int _tileBonus;

    // Factory upgrade level
    private int _level = 1;

    // Timer used to count production cooldown
    private float _timer;

    // If true, this factory is part of a combo and gets multiplied production
    private bool _comboActive = false;

    // Reference to GameManager to add resources
    private GameManager _gameManager;


    // Called when the factory is built
    public void Init(int tileBonus)
    {
        _tileBonus = tileBonus;

        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Factory could not find GameManager in the scene!");
        }

        // Apply sprite from FactoryData
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer != null && _data != null && _data.sprite != null)
        {
            renderer.sprite = _data.sprite;
        }
    }

    void Update()
    {
        // SAFETY CHECK
        // If _data was not assigned yet, avoid crashing
        if (_data == null)
            return;

        // Increase timer every frame
        _timer += Time.deltaTime;

        // When cooldown is reached → produce resources
        if (_timer >= _data.cooldown)
        {
            Produce();
            _timer = 0f;
        }
    }

void Produce()
{
    // Find the tile this factory belongs to
    Tile tile = GetComponentInParent<Tile>();

    if (tile == null)
    {
        Debug.LogError("Factory has no parent Tile!");
        return;
    }

    // If the tile is polluted, this factory produces nothing
    if (tile.GetTileType() == TileType.Pollution)
        return;

    // Safety check: GameManager must exist
    if (_gameManager == null)
    {
        Debug.LogError("Factory has no GameManager reference!");
        return;
    }

    // Safety check: FactoryData must exist
    if (_data == null)
    {
        Debug.LogError("Factory has no FactoryData assigned!");
        return;
    }

    // Base production + tile bonus
    int total = (_tileBonus * _level) + _data.baseProduction;

    // Apply combo multiplier if this factory is part of an active combo
    if (_comboActive)
    {
        total = Mathf.RoundToInt(total * _data.comboMultiplier);
    }

    // Give resources to the GameManager
    _gameManager.AddResources(total);
}

    // Called by ComboManager when a combo is detected
    public void ActivateCombo()
    {
        _comboActive = true;
    }


    // Used by ComboManager to know which pattern this factory uses
    public Vector2Int[] GetComboPattern()
    {
        if (_data == null)
        {
            Debug.LogError("Factory has no FactoryData assigned!");
            return null;
        }

        return _data.comboPattern;
    }


    // Called by Tile when building the factory
    // This assigns the ScriptableObject data
public void SetData(FactoryData data)
    {
        _data = data;

        if (_data == null)
        {
            Debug.LogError("SetData received NULL FactoryData");
        }
        else
        {
            Debug.Log("Factory data assigned: " + _data.name);
        }
    }
}