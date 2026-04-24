using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// with animation as well ( probably I wont remember)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class UIButtonHoverFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Sound")]
    [SerializeField] private AudioClip hoverSound;

    [Header("Scale Animation")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleDuration = 0.15f;

    private AudioSource _audio;
    private Tween _scaleTween;
    private Vector3 _originalScale;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _originalScale = transform.localScale; // save normal size
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // play sound
        if (_audio != null && hoverSound != null)
            _audio.PlayOneShot(hoverSound);

        // stop previous tween
        if (_scaleTween != null && _scaleTween.IsActive())
            _scaleTween.Kill();

        // scale up
        _scaleTween = transform.DOScale(_originalScale * hoverScale, scaleDuration)
            .SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // stop previous tween
        if (_scaleTween != null && _scaleTween.IsActive())
            _scaleTween.Kill();

        // scale back
        _scaleTween = transform.DOScale(_originalScale, scaleDuration)
            .SetUpdate(true);
    }

    void OnDestroy()
    {
        // clean tween if object is destroyed
        if (_scaleTween != null && _scaleTween.IsActive())
            _scaleTween.Kill();
    }
}