using UnityEngine;

public class Factory : MonoBehaviour
{
    [SerializeField] private FactoryData _data;
    [SerializeField] private ResourcePopup _resourcePopupPrefab;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem _upgradeEffectPrefab;
    [SerializeField] private Vector3 _upgradeEffectOffset = new Vector3(0f, 0.5f, 0f);


    //not exploit the same combo
    private bool _comboCompleted = false;

    private int _tileBonus;
    private int _level = 0; // 0 = first level in array
    private float _timer;
    private bool _comboActive = false;
    private GameManager _gameManager;

private FactoryIndicatorUI _indicatorUI;

    void Awake()
    {
        _indicatorUI = GetComponent<FactoryIndicatorUI>();
    }
    public void Init(int tileBonus)
    {
        _tileBonus = tileBonus;
        _gameManager = FindFirstObjectByType<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Factory could not find GameManager in the scene!");
        }

        ApplyCurrentLevelVisuals();
        CheckPollutionIndicator();
    }

    void Update()
    {
        if (_data == null) return;
        if (_gameManager != null && _gameManager.IsGameEnded) return;

        FactoryLevelData levelData = GetCurrentLevelData();
        if (levelData == null) return;

        _timer += Time.deltaTime;

        // produce when cooldown finishes
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

        // polluted tiles do not produce
        if (tile.GetTileType() == TileType.Pollution)
            return;

        if (_gameManager == null)
        {
            Debug.LogError("Factory has no GameManager reference!");
            return;
        }

        FactoryLevelData levelData = GetCurrentLevelData();
        if (levelData == null) return;

        // base production + terrain bonus
        int total = levelData.baseProduction + _tileBonus;

        // apply combo bonus
        if (_comboActive)
        {
            total = Mathf.RoundToInt(total * levelData.comboMultiplier);
        }

        _gameManager.AddResources(total);

        // show popup text
        if (_resourcePopupPrefab != null)
        {
            ResourcePopup.Create(_resourcePopupPrefab, transform.position + Vector3.up * 0.5f, total);
        }

        if (_gameManager != null && _gameManager.IsGameEnded)
            return;
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
            SpawnUpgradeEffect();
            if (_indicatorUI != null)
            _indicatorUI.ShowUpgradeArrow();
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
        FactoryLevelData levelData = GetCurrentLevelData();
        FactorySpriteAnimator animator = GetComponent<FactorySpriteAnimator>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (levelData == null) return;

        // use animator if script exists, otherwise use static sprite
        if (animator != null)
        {
            animator.ApplyLevelVisuals(levelData);
        }
        else if (renderer != null && levelData.sprite != null)
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

    void SpawnUpgradeEffect()
{
    if (_upgradeEffectPrefab == null) return;

    ParticleSystem effect = Instantiate(
        _upgradeEffectPrefab,
        transform.position + _upgradeEffectOffset,
        Quaternion.identity
    );

}

/////helpers for the tooltip
public FactoryData GetData()
{
    return _data;
}

public int GetLevel()
{
    return _level + 1;
}

public int GetTileBonusPublic()
{
    return _tileBonus;
}

public FactoryLevelData GetCurrentLevelDataPublic()
{
    return GetCurrentLevelData();
}

/// <summary>
/// tooltip name
/// </summary>
/// <returns></returns>
    public string GetDisplayName()
    {
        Tile tile = GetComponentInParent<Tile>();

        // if factory is on polluted tile
        if (tile != null && tile.GetTileType() == TileType.Pollution)
            return "POLLUTED FACTORY";

        // normal name
        if (_data != null && !string.IsNullOrEmpty(_data.factoryName))
            return _data.factoryName;

        return "Factory";
    }
/// arrows

void CheckPollutionIndicator()
{
    Tile tile = GetComponentInParent<Tile>();

    if (tile == null) return;

    if (tile.GetTileType() == TileType.Pollution)
    {
        if (_indicatorUI != null)
            _indicatorUI.ShowPollutionArrow();
    }
}
}