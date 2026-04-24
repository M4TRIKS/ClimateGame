using UnityEngine;
using DG.Tweening;
/// <summary>
/// /yoyo animation
/// </summary>
public class EventWarningFloat : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float moveDistance = 8f;
    [SerializeField] private float duration = 1.2f;

    private Vector3 _startPos;
    private Tween _floatTween;

    void Start()
    {
        _startPos = transform.localPosition;

        _floatTween = transform.DOLocalMoveY(
            _startPos.y + moveDistance,
            duration
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .SetUpdate(true);
    }

    void OnDestroy()
    {
        if (_floatTween != null && _floatTween.IsActive())
            _floatTween.Kill();
    }
}