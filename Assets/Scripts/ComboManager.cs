using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    //////////////////////////////////////////////////////////////////////
    // PATTERN DEFINITIONS
    //////////////////////////////////////////////////////////////////////

    // Vertical combo pattern (centered)
    private Vector2Int[] verticalPattern =
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };

    // Diagonal tiles to upgrade when vertical combo is triggered
    private Vector2Int[] diagonalTiles =
    {
        new Vector2Int(-1,2),
        new Vector2Int(1,2),
        new Vector2Int(-1,-2),
        new Vector2Int(1,-2)
    };

    // T pattern (stem center)
    private Vector2Int[] tPattern =
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,1),
        new Vector2Int(1,1)
    };

    //////////////////////////////////////////////////////////////////////
    // ENTRY POINT
    //////////////////////////////////////////////////////////////////////

    // Called every time a factory is built
    public void CheckCombos(Tile latestTile)
    {
        Vector2Int pos = latestTile.GetGridPosition();

        CheckVerticalCombo(pos);
        CheckTCombo(pos);
    }

    //////////////////////////////////////////////////////////////////////
    // GENERIC PATTERN CHECKER
    //////////////////////////////////////////////////////////////////////

    private bool CheckPattern(Vector2Int center, Vector2Int[] pattern)
    {
        foreach (Vector2Int offset in pattern)
        {
            Tile t = _gridManager.GetTileAtPosition(center + offset);

            if (!IsFactory(t))
                return false;
        }

        return true;
    }

    //////////////////////////////////////////////////////////////////////
    // VERTICAL COMBO
    //////////////////////////////////////////////////////////////////////

    private void CheckVerticalCombo(Vector2Int pos)
    {
        // Because the new factory could be anywhere in the pattern
        Vector2Int[] possibleCenters =
        {
            pos,
            pos + new Vector2Int(0,1),
            pos + new Vector2Int(0,-1),
            pos + new Vector2Int(1,2),
            pos + new Vector2Int(-1,2),
            pos + new Vector2Int(1,-2),
            pos + new Vector2Int(-1,-2)
        };

        foreach (Vector2Int center in possibleCenters)
        {
            if (CheckPattern(center, verticalPattern))
            {
                ApplyVerticalCombo(center);
            }
        }
    }

    private void ApplyVerticalCombo(Vector2Int center)
    {
        ////////////////////////////////////////////////////////////
        // UPGRADE DIAGONAL FACTORIES
        ////////////////////////////////////////////////////////////

        foreach (Vector2Int offset in diagonalTiles)
        {
            Tile t = _gridManager.GetTileAtPosition(center + offset);

            if (IsFactory(t))
            {
                t.CurrentFactory.Upgrade();
            }
        }

        ////////////////////////////////////////////////////////////
        // POLLUTION EFFECT
        ////////////////////////////////////////////////////////////

/*         Tile pollutionTop = _gridManager.GetTileAtPosition(center + new Vector2Int(0,2));
        Tile pollutionBot = _gridManager.GetTileAtPosition(center + new Vector2Int(0,-2));

        if (pollutionTop != null)
            pollutionTop.ConvertToPollution();

        if (pollutionBot != null)
            pollutionBot.ConvertToPollution(); */
    }

    //////////////////////////////////////////////////////////////////////
    // T COMBO
    //////////////////////////////////////////////////////////////////////

    private void CheckTCombo(Vector2Int pos)
    {
        Vector2Int[] possibleCenters =
        {
            pos,
            pos + new Vector2Int(0,-1),
            pos + new Vector2Int(1,-1),
            pos + new Vector2Int(-1,-1)
        };

        foreach (Vector2Int center in possibleCenters)
        {
            if (CheckPattern(center, tPattern))
            {
                ApplyTCombo(center);
            }
        }
    }

    private void ApplyTCombo(Vector2Int center)
    {
        Tile stem = _gridManager.GetTileAtPosition(center);

        if (IsFactory(stem))
        {
            stem.CurrentFactory.Upgrade();
        }

        
    }

    //////////////////////////////////////////////////////////////////////
    // HELPER
    //////////////////////////////////////////////////////////////////////

    private bool IsFactory(Tile t)
    {
        return t != null && t.IsBuilt();
    }
}