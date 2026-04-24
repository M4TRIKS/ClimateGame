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
        //helpers
        if (_factory == null)
            return new TooltipData("Factory", "Factory component missing");

        FactoryData data = _factory.GetData();

        if (data == null)
            return new TooltipData("Factory", "No FactoryData assigned");

        Tile tile = GetComponentInParent<Tile>();
        bool isPolluted = tile != null && tile.GetTileType() == TileType.Pollution;

        if (isPolluted)
        {
            return new TooltipData
            (
                $"<color=#7CFF4D><b> {_factory.GetDisplayName()}</b></color>",
                "This factory is built on polluted land.\n\n" +
                "<color=#FF5555><b>Production:</b> 0</color>\n\n" +
                "<color=#A6FF8C><i>No resources are generated.</i></color>",
                GetTooltipSprite(data)
            );
        }

        FactoryLevelData levelData = _factory.GetCurrentLevelDataPublic();

        string description = string.IsNullOrEmpty(data.factoryDescription)
            ? "No description"
            : data.factoryDescription;

        System.Text.StringBuilder body = new System.Text.StringBuilder();
        body.AppendLine(description);
        body.AppendLine();
        body.AppendLine($"<b>Level:</b> {_factory.GetLevel()}");

        if (levelData != null)
        {
            int totalProduction = levelData.baseProduction + _factory.GetTileBonusPublic();

            body.AppendLine($"<b>Base Production:</b> {levelData.baseProduction}");
            body.AppendLine($"<b>Total Production:</b> {totalProduction}");
            body.AppendLine($"<b>Cooldown:</b> {levelData.cooldown:0.##}s");
            body.AppendLine($"<b>Tile Bonus:</b> {_factory.GetTileBonusPublic()}");
            body.AppendLine($"<b>Combo Multiplier:</b> x{levelData.comboMultiplier:0.##}");
        }

        body.AppendLine($"<b>Combo Pattern:</b> {GetComboPatternText(data.comboPattern)}");

        return new TooltipData(
            _factory.GetDisplayName(),
            body.ToString(),
            GetTooltipSprite(data)
        );
    }
// not very easy to understand(VERY DIFFULT)
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
/// idk if i will use this but for the moment stays
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