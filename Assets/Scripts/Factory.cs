using UnityEngine;

public class Factory : MonoBehaviour
{
    [SerializeField] private FactoryData _data;

//not exploit the same combo
    private bool _comboCompleted = false;
    private int _tileBonus;
    private int _level = 0; // 0 = first level in array
    private float _timer;
    private bool _comboActive = false;
    private GameManager _gameManager;

    public void Init(int tileBonus)
    {
        _tileBonus = tileBonus;
        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Factory could not find GameManager in the scene!");
        }

        ApplyCurrentLevelVisuals();
    }

    void Update()
    {
        if (_data == null) return;

        FactoryLevelData levelData = GetCurrentLevelData();
        if (levelData == null) return;

        _timer += Time.deltaTime;

        if (_timer >= levelData.cooldown)
        {
            Produce();
            _timer = 0f;
        }
    }

    void Produce()
    {
        Tile tile = GetComponentInParent<Tile>();

        if (tile == null)
        {
            Debug.LogError("Factory has no parent Tile!");
            return;
        }

        if (tile.GetTileType() == TileType.Pollution)
            return;

        if (_gameManager == null)
        {
            Debug.LogError("Factory has no GameManager reference!");
            return;
        }

        FactoryLevelData levelData = GetCurrentLevelData();
        if (levelData == null) return;

        int total = levelData.baseProduction + _tileBonus;

        if (_comboActive)
        {
            total = Mathf.RoundToInt(total * levelData.comboMultiplier);
        }

        _gameManager.AddResources(total);
    }

    public void ActivateCombo()
    {
        _comboActive = true;
    }

    public void DeactivateCombo()
    {
        _comboActive = false;
    }

    public void Upgrade()
    {
        if (_data == null || _data.levels == null || _data.levels.Length == 0)
        {
            Debug.LogError("Factory has no upgrade levels configured!");
            return;
        }

        if (_level < _data.levels.Length - 1)
        {
            _level++;
            ApplyCurrentLevelVisuals();
            Debug.Log($"{name} upgraded to level {_level + 1}");
        }
        else
        {
            Debug.Log($"{name} is already at max level");
        }
    }

    FactoryLevelData GetCurrentLevelData()
    {
        if (_data == null)
        {
            Debug.LogError("Factory has no FactoryData assigned!");
            return null;
        }

        if (_data.levels == null || _data.levels.Length == 0)
        {
            Debug.LogError($"FactoryData '{_data.name}' has no levels configured!");
            return null;
        }

        int clampedLevel = Mathf.Clamp(_level, 0, _data.levels.Length - 1);
        return _data.levels[clampedLevel];
    }

    void ApplyCurrentLevelVisuals()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        FactoryLevelData levelData = GetCurrentLevelData();

        if (renderer != null && levelData != null && levelData.sprite != null)
        {
            renderer.sprite = levelData.sprite;
        }
    }

    public Vector2Int[] GetComboPattern()
    {
        if (_data == null)
        {
            Debug.LogError("Factory has no FactoryData assigned!");
            return null;
        }

        return _data.comboPattern;
    }

    public void SetData(FactoryData data)
    {
        _data = data;
        _level = 0;
        _comboActive = false;

        if (_data == null)
        {
            Debug.LogError("SetData received NULL FactoryData");
        }
        else
        {
            Debug.Log("Factory data assigned: " + _data.name);
        }
    }
    
    public bool HasCompletedCombo()
{
    return _comboCompleted;
}

public void MarkComboCompleted()
{
    _comboCompleted = true;
}
}