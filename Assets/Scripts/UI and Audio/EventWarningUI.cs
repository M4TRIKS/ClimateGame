using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventWarningUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    [SerializeField] private Image _backDetailImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _eventImage;

    [Header("Background Sprites")]
    [SerializeField] private Sprite _backDetailSprite;
    [SerializeField] private Sprite _frameSprite;

    [Header("Event Sprites")]
    [SerializeField] private Sprite _fireSprite;
    [SerializeField] private Sprite _floodSprite;

    [Header("Descriptions")]
    [TextArea(2,5)]
    [SerializeField] private string _fireDescription =
        "Fire has started. It spreads to nearby tiles and destroys factories.";

    [TextArea(2,5)]
    [SerializeField] private string _floodDescription =
        "Flood has started. Water spreads and destroys factories.";

    private string _currentTitle;
    private string _currentDescription;
    private bool _isVisible;

    void Awake()
    {
        HideEventWarning();
    }

    public void ShowEventWarning(RandomEventManager.EventType eventType)
    {
        _isVisible = true;
        gameObject.SetActive(true);

        // always show frame/background
        if (_backDetailImage != null)
        {
            _backDetailImage.gameObject.SetActive(true);
            _backDetailImage.sprite = _backDetailSprite;
        }
        if (_backgroundImage != null)
            _backgroundImage.sprite = _frameSprite;
            

        switch (eventType)
        {
            case RandomEventManager.EventType.Fire:

                if (_eventImage != null)
                    _eventImage.sprite = _fireSprite;

                _currentTitle = "Fire Event";
                _currentDescription = _fireDescription;
                break;

            case RandomEventManager.EventType.Flood:

                if (_eventImage != null)
                    _eventImage.sprite = _floodSprite;

                _currentTitle = "Flood Event";
                _currentDescription = _floodDescription;
                break;
        }
    }

    public void HideEventWarning()
    {
        _isVisible = false;
        TooltipUI.Hide_Static();
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isVisible) return;

        TooltipUI.Show_Static(new TooltipData(
            _currentTitle,
            _currentDescription,
            _eventImage.sprite
        ));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Hide_Static();
    }
}