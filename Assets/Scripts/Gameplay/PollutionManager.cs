using UnityEngine;

public class PollutionManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void ApplyFactoryPollution(Tile factoryTile, float pollutionAmount)
{
    //gets the factory created and stores the neighbors
    Vector2Int center = factoryTile.GetGridPosition();

    Vector2Int[] neighbors =
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,1),
        new Vector2Int(-1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,-1)
    };

    foreach (Vector2Int offset in neighbors)
    {
        Tile t = _gridManager.GetTileAtPosition(center + offset);

        if (t != null)
        {
            t.AddPollutionChance(pollutionAmount);
        }
    }
}
}
