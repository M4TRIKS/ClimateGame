using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] private Texture2D _normalCursor;
    [SerializeField] private Texture2D _pressedCursor;
    [SerializeField] private Vector2 _hotspot = Vector2.zero;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SetNormalCursor();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SetPressedCursor();

        if (Input.GetMouseButtonUp(0))
            SetNormalCursor();
    }

    public void SetNormalCursor()
    {
        Cursor.SetCursor(_normalCursor, _hotspot, CursorMode.Auto);
    }

    public void SetPressedCursor()
    {
        Cursor.SetCursor(_pressedCursor, _hotspot, CursorMode.Auto);
    }
}