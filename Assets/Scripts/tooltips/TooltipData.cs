using UnityEngine;

public class TooltipData
{
    public string Title;
    public string Body;
    public Sprite Icon;

    public TooltipData(string title, string body, Sprite icon = null)
    {
        Title = title;
        Body = body;
        Icon = icon;
    }
}