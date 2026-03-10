using UnityEngine;

public class Factory : MonoBehaviour
{   private PollutionManager _pollutionManager;
    [Header("generation")]
    [SerializeField] private int _baseProduction = 1;
    [SerializeField] private float _cooldown = 5f;
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer _renderer; 
    [SerializeField] private Sprite _upgradedSprite;   

    private int _level = 1;
    private int _tileBonus;
    // timer used to count time until next production
    private float _timer;
    private GameManager _gameManager;

    /// This function is called right after the factory is spawned.
    /// The tile sends the bonus depending on type
 /*    public void Init(int tileBonus)
    {
        _tileBonus = tileBonus;
        _gameManager = FindFirstObjectByType<GameManager>();
    } */
 public void Init(int tileBonus, GameManager gameManager, PollutionManager pollutionManager)
        {
            _tileBonus = tileBonus;
            _gameManager = gameManager;
            _pollutionManager = pollutionManager;
        }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _cooldown)
        {
            Produce();
            _timer = 0f;
        }

    }

    /// Calculates how many resources the factory produces
    /// and sends them to the GameManager
    
    void Produce()
    {
        
        int total = _tileBonus * _baseProduction  ;

        // Give resources to the GameManager
        _gameManager.AddResources(total);
    }

public void Upgrade()
    {
        // Don't upgrade if it's already level 2
        if (_level > 1) return; 

        _level++;
        _baseProduction += 1;
        _cooldown *= 0.9f; //multiply will get 10 %faster 

        //skin
        if (_renderer != null && _upgradedSprite != null)
        {
            _renderer.sprite = _upgradedSprite;
        }
        PollutionManager pollution = FindFirstObjectByType<PollutionManager>();
    
        Tile tile = GetComponentInParent<Tile>();
      // Apply pollution to the tiles around this factory.
        // 0.2f = 20% pollution chance increase.
        // Upgraded factories pollute more than normal factories.
        if (tile != null)
        {
            pollution.ApplyFactoryPollution(tile, 0.05f);
        }
        
        Debug.Log("upgraded");
    }
}