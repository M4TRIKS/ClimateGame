using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipWarningUI : MonoBehaviour
{
    public static TooltipWarningUI Instance { get; private set; }
    public static bool IsShowing => Instance != null && Instance._isVisible;

    public enum WarningType
    {
        Default,
        NotEnoughMoney,
        Water,
        Occupied,
        Fire
    }

    [Header("References")]
    [SerializeField] private RectTransform _backgroundRect;
    [SerializeField] private TextMeshProUGUI _warningText;
    [SerializeField] private Image _backgroundImage;

    [Header("Settings")]
    [SerializeField] private float _defaultDuration = 1.2f;
    [SerializeField] private float _flashSpeed = 10f;

    [Header("Colors")]
    [SerializeField] private Color _defaultTextColor = Color.white;
    [SerializeField] private Color _moneyTextColor = Color.red;
    [SerializeField] private Color _waterTextColor = new Color(0.3f, 0.8f, 1f);
    [SerializeField] private Color _occupiedTextColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color _fireTextColor = new Color(1f, 0.45f, 0.1f);

    [Header("Background Colors")]
    [SerializeField] private Color _defaultBackgroundColor = new Color(0f, 0f, 0f, 0.85f);
    [SerializeField] private Color _moneyBackgroundColor = new Color(0.25f, 0f, 0f, 0.9f);
    [SerializeField] private Color _waterBackgroundColor = new Color(0f, 0.15f, 0.25f, 0.9f);
    [SerializeField] private Color _occupiedBackgroundColor = new Color(0.25f, 0.2f, 0f, 0.9f);
    [SerializeField] private Color _fireBackgroundColor = new Color(0.25f, 0.08f, 0f, 0.9f);

    private float _timer;
    private bool _isVisible;
    private Color _baseTextColor;
    private Color _baseBackgroundColor;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupTextOutline();
        Hide();
    }

    void Update()
    {
        if (!_isVisible) return;

        _timer -= Time.unscaledDeltaTime;
        if (_timer <= 0f)
        {
            Hide();
            return;
        }

        FlashVisuals();
    }

    void SetupTextOutline()
    {
        if (_warningText == null) return;

        _warningText.fontMaterial.EnableKeyword("OUTLINE_ON");
        _warningText.outlineWidth = 0.2f;
        _warningText.outlineColor = Color.black;

        var margins = _warningText.margin;
        margins.x = 0;
        margins.y = 0;
        margins.z = 0;
        margins.w = 0;
        _warningText.margin = margins;
    }

    void FlashVisuals()
    {
        float flash = Mathf.Abs(Mathf.Sin(Time.unscaledTime * _flashSpeed));

        Color textColor = _baseTextColor;
        textColor.a = Mathf.Lerp(0.45f, 1f, flash);
        _warningText.color = textColor;

        if (_backgroundImage != null)
        {
            Color bgColor = _baseBackgroundColor;
            bgColor.a = Mathf.Lerp(_baseBackgroundColor.a * 0.65f, _baseBackgroundColor.a, flash);
            _backgroundImage.color = bgColor;
        }
    }

    public void Show(string text, WarningType warningType = WarningType.Default, float duration = -1f)
    {
        TooltipUI.Hide_Static();

        gameObject.SetActive(true);
        _isVisible = true;
        _timer = duration > 0f ? duration : _defaultDuration;

        _warningText.text = text;
        ApplyWarningStyle(warningType);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_backgroundRect);
    }

    void ApplyWarningStyle(WarningType warningType)
    {
        switch (warningType)
        {
            case WarningType.NotEnoughMoney:
                _baseTextColor = _moneyTextColor;
                _baseBackgroundColor = _moneyBackgroundColor;
                break;

            case WarningType.Water:
                _baseTextColor = _waterTextColor;
                _baseBackgroundColor = _waterBackgroundColor;
                break;

            case WarningType.Occupied:
                _baseTextColor = _occupiedTextColor;
                _baseBackgroundColor = _occupiedBackgroundColor;
                break;

            case WarningType.Fire:
                _baseTextColor = _fireTextColor;
                _baseBackgroundColor = _fireBackgroundColor;
                break;

            default:
                _baseTextColor = _defaultTextColor;
                _baseBackgroundColor = _defaultBackgroundColor;
                break;
        }

        _warningText.color = _baseTextColor;

        if (_backgroundImage != null)
            _backgroundImage.color = _baseBackgroundColor;
    }

    public void Hide()
    {
        _isVisible = false;
        gameObject.SetActive(false);
    }

    public static void Show_Static(string text, WarningType warningType = WarningType.Default, float duration = -1f)
    {
        if (Instance != null)
            Instance.Show(text, warningType, duration);
    }

    public static void Hide_Static()
    {
        if (Instance != null)
            Instance.Hide();
    }
}