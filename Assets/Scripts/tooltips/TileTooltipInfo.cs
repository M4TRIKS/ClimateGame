using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TileTooltipInfo : MonoBehaviour
{
    private Tile _tile;

    void Awake()
    {
        _tile = GetComponent<Tile>();
    }

    public TooltipData GetTooltipData()
    {
        if (_tile == null)
            return new TooltipData("Tile", "");

        TileType type = _tile.GetTileType();

        string title = GetTileName(type);
        string body = GetTileBody(type);

        return new TooltipData(title, body);
    }

    string GetTileName(TileType type)
    {
        switch (type)
        {
            case TileType.Grass: return "Grass";
            case TileType.Sand: return "Sand";
            case TileType.Rock: return "Rock";
            case TileType.Water: return "Water";
            case TileType.Fire: return "Fire";
            case TileType.Pollution: return "Pollution";
        }

        return "Tile";
    }

    string GetTileBody(TileType type)
    {
        switch (type)
        {
            case TileType.Grass:
            case TileType.Sand:
            case TileType.Rock:
                return $"Bonus: {_tile.GetTileBonus()}";

            case TileType.Water:
            case TileType.Fire:
            case TileType.Pollution:
                return "";

            default:
                return "";
        }
    }
}