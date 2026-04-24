using UnityEngine;
using DG.Tweening;

public class ArrowYoyo : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float _moveDistance = 0.12f;
    [SerializeField] private float _duration = 0.6f;

    private Vector3 _startLocalPosition;
    private Tween _tween;

    void OnEnable()
    {
        _startLocalPosition = transform.localPosition;

        _tween = transform.DOLocalMoveY(
            _startLocalPosition.y + _moveDistance,
            _duration
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();

        transform.localPosition = _startLocalPosition;
    }

    void OnDestroy()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();
    }
}