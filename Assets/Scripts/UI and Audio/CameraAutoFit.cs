using UnityEngine;

public class CameraAutoFit : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private float _padding = 1f;
    [SerializeField] private float _zPosition = -10f;

    void Start()
    {
        FitCamera();
    }

    void FitCamera()
    {
        Camera cam = Camera.main;
        if (cam == null || _gridManager == null) return;

        float gridWidth = _gridManager.Width;
        float gridHeight = _gridManager.Height;

        // center camera on grid
        cam.transform.position = new Vector3(
            gridWidth / 2f - 0.5f,
            gridHeight / 2f - 0.5f,
            _zPosition
        );

        // calculate size needed to fit height
        float verticalSize = gridHeight / 2f;

        // calculate size needed to fit width
        float horizontalSize = gridWidth / (2f * cam.aspect);

        // use whichever is bigger, so full grid always fits
        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize) + _padding;
    }
}