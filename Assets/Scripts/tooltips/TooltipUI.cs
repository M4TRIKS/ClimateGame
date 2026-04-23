using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _rootRect;
    [SerializeField] private RectTransform _backgroundRect;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _bodyText;
    [SerializeField] private Image _iconImage;

    [Header("Settings")]
    [SerializeField] private Vector2 _mouseOffset = new Vector2(20f, 20f);
    [SerializeField] private float _maxWidth = 222f;
    [SerializeField] private float _screenPadding = 12f;

    private bool _isVisible;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (_canvas == null)
            _canvas = GetComponentInParent<Canvas>();

        if (_canvasRect == null && _canvas != null)
            _canvasRect = _canvas.GetComponent<RectTransform>();

        if (_rootRect == null)
            _rootRect = GetComponent<RectTransform>();

        Hide();
    }

    void LateUpdate()
    {
        if (!_isVisible) return;

        FollowMouse();
        ClampToCanvas();
    }

    public void Show(TooltipData data)
    {
        if (TooltipWarningUI.IsShowing)
            return;

        if (data == null) return;

        gameObject.SetActive(true);
        _isVisible = true;

        _titleText.text = data.Title;
        _bodyText.text = data.Body;

        if (_iconImage != null)
        {
            bool hasIcon = data.Icon != null;
            _iconImage.gameObject.SetActive(hasIcon);

            if (hasIcon)
                _iconImage.sprite = data.Icon;
        }

        // limit body width so wrapping works
        _bodyText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            _maxWidth
        );

        // rebuild layout so Content Size Fitter updates background size
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_backgroundRect);

        // background must sit exactly on the root
        _backgroundRect.anchoredPosition = Vector2.zero;

        FollowMouse();
        ClampToCanvas();
    }

    public void Hide()
    {
        _isVisible = false;
        gameObject.SetActive(false);
    }

    void FollowMouse()
    {
        float scaleFactor = _canvas != null ? _canvas.scaleFactor : 1f;

        Vector2 mouseCanvasPos = (Vector2)Input.mousePosition / scaleFactor;
        _rootRect.anchoredPosition = mouseCanvasPos + _mouseOffset;

        _backgroundRect.anchoredPosition = Vector2.zero;
    }

    void ClampToCanvas()
    {
        Vector2 pos = _rootRect.anchoredPosition;
        Vector2 bgSize = _backgroundRect.rect.size;
        Vector2 canvasSize = _canvasRect.rect.size;

        float minX = _screenPadding;
        float minY = _screenPadding;

        float maxX = canvasSize.x - bgSize.x - _screenPadding;
        float maxY = canvasSize.y - bgSize.y - _screenPadding;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _rootRect.anchoredPosition = pos;
    }

    public static void Show_Static(TooltipData data)
    {
        if (Instance != null)
            Instance.Show(data);
    }

    public static void Hide_Static()
    {
        if (Instance != null)
            Instance.Hide();
    }
}