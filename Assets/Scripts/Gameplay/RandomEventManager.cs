using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public enum EventType
    {
        None,
        Fire,
        Flood
    }

    [Header("References")]
    [SerializeField] private EventWarningUI _eventWarningUI;
    [SerializeField] private GridManager _gridManager;

    [Header("Event Settings")]
    [SerializeField] private EventType _selectedEvent = EventType.Fire;

    [Header("Fire Settings")]
    [SerializeField] private float _fireStartDelay = 20f;
    [SerializeField] private float _fireSpreadDelay = 15f;

    [Header("Flood Settings")]
    [SerializeField] private float _floodStartDelay = 20f;
    [SerializeField] private float _floodSpreadDelay = 25f;

    private readonly List<Tile> _fireTiles = new List<Tile>();
    private bool _eventStarted = false;

    void Start()
    {
        PickRandomEvent();
        StartCoroutine(RunSelectedEvent());
    }

    void PickRandomEvent()
    {
        EventType[] possibleEvents =
        {
            EventType.Fire,
            EventType.Flood
        };

        // choose random disaster
        _selectedEvent = possibleEvents[Random.Range(0, possibleEvents.Length)];
        Debug.Log("Selected event: " + _selectedEvent);
    }

    IEnumerator RunSelectedEvent()
    {
        // wait and start chosen event
        if (_selectedEvent == EventType.Fire)
        {
            yield return new WaitForSeconds(_fireStartDelay);
            StartFireEvent();
        }
        else if (_selectedEvent == EventType.Flood)
        {
            yield return new WaitForSeconds(_floodStartDelay);
            if (_eventWarningUI != null)
            _eventWarningUI.ShowEventWarning(EventType.Flood);
            //why did I use coroutine here but not in fireEvent?
            StartCoroutine(SpreadFloodRoutine());
        }
    }

    void StartFireEvent()
    {
 
        if (_eventStarted) return;
        _eventStarted = true;
        if (_eventWarningUI != null)
        _eventWarningUI.ShowEventWarning(EventType.Fire);
        

        Tile firstFireTile = GetRandomValidFireStartTile();

        if (firstFireTile == null)
        {
            Debug.LogWarning("No valid tile found to start fire.");
            return;
        }

        firstFireTile.ConvertToFire();
        _fireTiles.Add(firstFireTile);

        StartCoroutine(SpreadFireRoutine());
    }

    IEnumerator SpreadFireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_fireSpreadDelay);

            Tile nextTile = GetNextSpreadTileFromFireChain();

            if (nextTile != null)
            {
                nextTile.ConvertToFire();
                _fireTiles.Add(nextTile);
            }
            else
            {
                Debug.Log("Fire cannot spread anymore.");
                yield break;
            }
        }
    }

    Tile GetNextSpreadTileFromFireChain()
    {
        // check last fire tiles first
        for (int i = _fireTiles.Count - 1; i >= 0; i--)
        {
            Tile sourceTile = _fireTiles[i];

            if (sourceTile == null)
                continue;

            Tile candidate = GetRandomNeighbor(sourceTile);

            if (candidate != null)
            {
                return candidate;
            }
        }

        return null;
    }

    Tile GetRandomValidFireStartTile()
    {
        List<Tile> validTiles = new List<Tile>();

        foreach (Tile tile in _gridManager.GetAllTiles())
        {
            if (tile != null && tile.GetTileType() != TileType.Water && tile.GetTileType() != TileType.Fire)
            {
                validTiles.Add(tile);
            }
        }

        if (validTiles.Count == 0)
            return null;

        return validTiles[Random.Range(0, validTiles.Count)];
    }

    Tile GetRandomNeighbor(Tile centerTile)
    {
        if (centerTile == null) return null;

        Vector2Int center = centerTile.GetGridPosition();

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        ShuffleDirections(directions);

        foreach (Vector2Int dir in directions)
        {
            Tile neighbor = _gridManager.GetTileAtPosition(center + dir);

            if (neighbor != null &&
                neighbor.GetTileType() != TileType.Water &&
                neighbor.GetTileType() != TileType.Fire)
            {
                return neighbor;
            }
        }

        return null;
    }

    IEnumerator SpreadFloodRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_floodSpreadDelay);

            Tile nextFloodTile = GetNextFloodTile();

            if (nextFloodTile != null)
            {
                nextFloodTile.ConvertToWater();
            }
            else
            {
                Debug.Log("Flood cannot spread anymore.");
                yield break;
            }
        }
    }

    Tile GetNextFloodTile()
    {
        List<Tile> waterTiles = new List<Tile>();

        // collect all current water tiles
        foreach (Tile tile in _gridManager.GetAllTiles())
        {
            if (tile != null && tile.GetTileType() == TileType.Water)
            {
                waterTiles.Add(tile);
            }
        }

        ShuffleTiles(waterTiles);

        foreach (Tile waterTile in waterTiles)
        {
            Tile candidate = GetRandomFloodNeighbor(waterTile);

            if (candidate != null)
            {
                return candidate;
            }
        }

        return null;
    }

    Tile GetRandomFloodNeighbor(Tile centerTile)
    {
        if (centerTile == null) return null;

        Vector2Int center = centerTile.GetGridPosition();

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        ShuffleDirections(directions);

        foreach (Vector2Int dir in directions)
        {
            Tile neighbor = _gridManager.GetTileAtPosition(center + dir);

            if (neighbor != null && neighbor.GetTileType() != TileType.Water)
            {
                return neighbor;
            }
        }

        return null;
    }

    void ShuffleDirections(List<Vector2Int> list)
    {
        // randomize direction order
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Vector2Int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ShuffleTiles(List<Tile> list)
    {
        // randomize tile order
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Tile temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}