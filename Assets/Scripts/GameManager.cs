using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
        [Header("Managers")]

    [SerializeField] private ComboManager _comboManager;
    [SerializeField] private PollutionManager  _pollutionManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private TextMeshProUGUI _resourceText;
    [SerializeField] private TextMeshProUGUI _resourceTargetText;
[Header("resources")]

    [SerializeField] private int _targetResources = 5000;

    [SerializeField] private int _factoryCost = 25;
    [SerializeField] private int _resources = 15;
    public bool _gameEnded = false;


    void Start()
    {
        _resourceTargetText.text = "Target: " + _targetResources;
        UpdateUI();
    }
    // Update is called once per frame
    
  
public void AddResources(int amount)
{
    _resources += amount;
    UpdateUI();
    CheckWinCondition();
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

    //checksfor combos
    _comboManager.CheckCombos(tile);
    
    //adds pollution
    _pollutionManager.ApplyFactoryPollution(tile, 0.025f);

    return true;
}

void UpdateUI()
{
    _resourceText.text = "Energy: " + _resources;
    
}

void CheckWinCondition()
{
    if (_resources >= _targetResources && !_gameEnded)
    {
        _gameEnded = true;
        Debug.Log("you win yipeeee!");
    }
}
public void TimeUp()
{
    if (_gameEnded) return; //if the plaayer gets to the goal just in the moment

    _gameEnded = true;

    if (_resources >= _targetResources) //checks one last time
        Debug.Log("WIN!");
    else
        Debug.Log("LOSE!");
}


}


