

using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

  public void CheckFactoryCombo(Tile centerTile)
    {
        if (centerTile == null || _gridManager == null)
            return;

        Factory centerFactory = centerTile.CurrentFactory;

        if (centerFactory == null)
            return;

        Vector2Int[] pattern = centerFactory.GetComboPattern();

        if (pattern == null || pattern.Length == 0)
            return;

        Vector2Int centerPos = centerTile.GetGridPosition();

        foreach (Vector2Int offset in pattern)
        {
            Tile t = _gridManager.GetTileAtPosition(centerPos + offset);

            if (t == null || t.CurrentFactory == null)
                return;
        }

        // Activate center
        centerFactory.ActivateCombo();

        // Activate all other factories in the pattern
        foreach (Vector2Int offset in pattern)
        {
            Tile t = _gridManager.GetTileAtPosition(centerPos + offset);
            t.CurrentFactory.ActivateCombo();
        }
    }
}