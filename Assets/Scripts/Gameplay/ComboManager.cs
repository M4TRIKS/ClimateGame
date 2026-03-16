using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    public void CheckAllCombos()
    {
        if (_gridManager == null) return;

        // check combo for every tile
        foreach (Tile tile in _gridManager.GetAllTiles())
        {
            CheckFactoryCombo(tile);
        }
    }

    public void CheckFactoryCombo(Tile centerTile)
    {
        if (centerTile == null || _gridManager == null)
            return;

        Factory centerFactory = centerTile.CurrentFactory;

        if (centerFactory == null)
            return;

        // don't repeat same combo
        if (centerFactory.HasCompletedCombo())
            return;

        Vector2Int[] pattern = centerFactory.GetComboPattern();

        if (pattern == null || pattern.Length == 0)
            return;

        Vector2Int centerPos = centerTile.GetGridPosition();

        // check if all required tiles have factories
        foreach (Vector2Int offset in pattern)
        {
            Tile t = _gridManager.GetTileAtPosition(centerPos + offset);

            if (t == null || t.CurrentFactory == null)
                return;
        }

        // combo complete: activate and upgrade center
        centerFactory.ActivateCombo();
        centerFactory.Upgrade();
        centerFactory.MarkComboCompleted();

        // activate and upgrade neighbor factories
        foreach (Vector2Int offset in pattern)
        {
            Tile t = _gridManager.GetTileAtPosition(centerPos + offset);
            t.CurrentFactory.ActivateCombo();
            t.CurrentFactory.Upgrade();
        }
    }
}