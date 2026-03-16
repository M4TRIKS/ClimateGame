using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ComboManager _comboManager;
    [SerializeField] private PollutionManager _pollutionManager;
    [SerializeField] private GridManager _gridManager;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _resourceText;
    [SerializeField] private TextMeshProUGUI _resourceTargetText;
    [SerializeField] private Slider _targetBar;

    [Header("End game UI")]
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private Image _endGameImage;
    [SerializeField] private Sprite _winSprite;
    [SerializeField] private Sprite _loseSprite;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;

    [Header("Resources")]
    [SerializeField] private int _baseTargetResources = 5000;
    [SerializeField] private int _factoryCost = 25;
    [SerializeField] private int _startingResources = 15;

    private static int s_currentRound = 1;
    private int _targetResources;
    private int _resources;
    public bool _gameEnded = false;

    // Progression that survives scene reload
    private static int s_currentTarget = 5000;
    private static int s_nextIncrease = 1000;
    private static bool s_initialized = false;

    public bool IsGameEnded => _gameEnded;

    void Awake()
    {
        // only initialize static progression once
        if (!s_initialized)
        {
            s_currentTarget = _baseTargetResources;
            s_nextIncrease = 1000;
            s_initialized = true;
        }

        _targetResources = s_currentTarget;
        _resources = _startingResources;
    }

    void Start()
    {
        if (_resourceTargetText != null)
            _resourceTargetText.text = "Target " + _targetResources;

        UpdateUI();
        SetupEndGameUI();
    }

    void SetupEndGameUI()
    {
        // hide end panel at start
        if (_endGamePanel != null)
            _endGamePanel.SetActive(false);

        if (_continueButton != null)
        {
            _continueButton.gameObject.SetActive(false);
        }

        if (_restartButton != null)
        {
            _restartButton.gameObject.SetActive(false);
        }
    }

    public void AddResources(int amount)
    {
        if (_gameEnded) return;

        _resources += amount;
        UpdateUI();
        CheckWinCondition();
    }

    public bool TryBuild(Tile tile, FactoryData factoryData)
    {
        if (_gameEnded) return false;
        if (tile == null) return false;

        if (factoryData == null)
        {
            Debug.LogError("TryBuild received null FactoryData");
            return false;
        }

        if (!tile.CanBuild()) return false;

        if (_resources < _factoryCost)
        {
            Debug.Log("Not enough resources");
            return false;
        }

        // place factory
        tile.Build(factoryData);
        _resources -= _factoryCost;
        UpdateUI();

        // check combos after build
        if (_comboManager != null)
        {
            _comboManager.CheckAllCombos();
        }

        // apply pollution after build
        if (_pollutionManager != null)
        {
            _pollutionManager.ApplyFactoryPollution(tile, 0.025f); // should I add more pollution?
        }

        return true;
    }

    void UpdateUI()
    {
        if (_resourceText != null)
            _resourceText.text =  _resources + "$";

        if (_resourceTargetText != null)
            _resourceTargetText.text = "Target " + _targetResources +"$";

        if (_targetBar != null)
            _targetBar.value = (float)_resources / _targetResources;
    }

    void CheckWinCondition()
    {
        if (_resources >= _targetResources && !_gameEnded)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        _gameEnded = true;
        ShowEndScreen(true);
        Debug.Log("WIN!");
    }

    public void TimeUp()
    {
        if (_gameEnded) return;

        _gameEnded = true;

        // if time ends, check if player reached target
        if (_resources >= _targetResources)
        {
            ShowEndScreen(true);
            Debug.Log("WIN!");
        }
        else
        {
            ShowEndScreen(false);
            Debug.Log("LOSE!");
        }
    }

    void ShowEndScreen(bool won)
    {
        if (_endGamePanel != null)
            _endGamePanel.SetActive(true);

        if (_endGameImage != null)
            _endGameImage.sprite = won ? _winSprite : _loseSprite;

        // continue only if won
        if (_continueButton != null)
            _continueButton.gameObject.SetActive(won);

        // restart only if lost
        if (_restartButton != null)
            _restartButton.gameObject.SetActive(!won);
    }

    public void ContinueGame()
    {
        // go to next round and increase target
        s_currentRound++;
        s_currentTarget += s_nextIncrease;
        s_nextIncrease *= 2;

        ReloadCurrentScene();
    }

    public void RestartFromBeginning()
    {
        // reset progression
        s_currentRound = 1;
        s_currentTarget = _baseTargetResources;
        s_nextIncrease = 1000;

        ReloadCurrentScene();
    }

    void ReloadCurrentScene()
    {
        SceneLoader.Instance.ReloadScene();
    }

    ///
    public static int GetCurrentRound()
    {
        return s_currentRound;
    }
}