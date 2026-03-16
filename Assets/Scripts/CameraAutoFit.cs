using UnityEngine;

public class CameraAutoFit : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private float _padding = 1f;

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

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = gridWidth / gridHeight;

        if (screenRatio >= targetRatio)
        {
            // screen is wider, so fit height
            cam.orthographicSize = gridHeight / 2f + _padding;
        }
        else
        {
            // screen is taller/narrower, so fit width too
            float difference = targetRatio / screenRatio;
            cam.orthographicSize = (gridHeight / 2f * difference) + _padding;
        }
    }
}