using UnityEngine;

[RequireComponent(typeof(FactoryTooltipInfo))]
public class FactoryTooltipTrigger : MonoBehaviour
{
    private FactoryTooltipInfo _factoryTooltipInfo;
    private Factory _factory;

    private bool _showingFullTooltip = false;
    private bool _isHovered = false;

    void Awake()
    {
        _factoryTooltipInfo = GetComponent<FactoryTooltipInfo>();
        _factory = GetComponent<Factory>();
    }

    void OnMouseEnter()
    {
        _isHovered = true;

        if (FactoryManager.IsDraggingFactory) return;
        if (TooltipWarningUI.IsShowing) return;
        if (_showingFullTooltip) return;

        ShowShortTooltip();
    }

    void OnMouseExit()
    {
        _isHovered = false;
        _showingFullTooltip = false;
        TooltipUI.Hide_Static();
    }

    void OnMouseOver()
    {
        if (FactoryManager.IsDraggingFactory) return;
        if (TooltipWarningUI.IsShowing) return;
        if (!_isHovered) return;

        if (Input.GetMouseButtonDown(1))
        {
            _showingFullTooltip = true;
            ShowFullTooltip();
        }
    }

    void OnMouseDown()
    {
        TooltipUI.Hide_Static();
    }

    void ShowShortTooltip()
    {
        if (_factory == null || _factory.GetData() == null)
        {
            TooltipUI.Show_Static(new TooltipData("Factory", "Right click info"));
            return;
        }

        string title = string.IsNullOrEmpty(_factory.GetData().factoryName)
            ? "Factory"
            : _factory.GetData().factoryName;

        TooltipUI.Show_Static(new TooltipData(
            title,
            "<i>Right click info</i>"
        ));
    }

    void ShowFullTooltip()
    {
        if (_factoryTooltipInfo != null)
        {
            TooltipUI.Show_Static(_factoryTooltipInfo.GetTooltipData());
        }
    }
}