using System.Text;
using UnityEngine;

[RequireComponent(typeof(Factory))]
public class FactoryTooltipInfo : MonoBehaviour
{
    private Factory _factory;

    void Awake()
    {
        _factory = GetComponent<Factory>();
    }

    public TooltipData GetTooltipData()
    {
        if (_factory == null || _factory.GetData() == null)
            return new TooltipData("Factory", "No information available");

        FactoryData data = _factory.GetData();
        FactoryLevelData levelData = _factory.GetCurrentLevelDataPublic();

        string description = string.IsNullOrEmpty(data.factoryDescription)
            ? "No description"
            : data.factoryDescription;

        StringBuilder body = new StringBuilder();
        body.AppendLine(description);
        body.AppendLine();
        body.AppendLine($"<b>Level:</b> {_factory.GetLevel()}");

        if (levelData != null)
        {
            body.AppendLine($"<b>Production:</b> {levelData.baseProduction}");
            body.AppendLine($"<b>Cooldown:</b> {levelData.cooldown:0.##}s");
            body.AppendLine($"<b>Tile Bonus:</b> {_factory.GetTileBonusPublic()}");
            body.AppendLine($"<b>Combo Multiplier:</b> x{levelData.comboMultiplier:0.##}");
        }

        body.AppendLine($"<b>Combo Pattern:</b> {GetComboPatternText(data.comboPattern)}");

        return new TooltipData(
            data.factoryName,
            body.ToString(),
            GetTooltipSprite(data)
        );
    }

    string GetComboPatternText(Vector2Int[] pattern)
    {
        if (pattern == null || pattern.Length == 0)
            return "None";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < pattern.Length; i++)
        {
            sb.Append($"({pattern[i].x}, {pattern[i].y})");

            if (i < pattern.Length - 1)
                sb.Append(", ");
        }

        return sb.ToString();
    }

    Sprite GetTooltipSprite(FactoryData data)
    {
        if (data == null) return null;

        if (data.tooltipSprite != null)
            return data.tooltipSprite;

        if (data.levels != null &&
            data.levels.Length > 0 &&
            data.levels[0].sprite != null)
        {
            return data.levels[0].sprite;
        }

        return null;
    }
}