using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class UIButtonTween : MonoBehaviour
{
    [SerializeField] private float punchStrength = 0.25f;
    [SerializeField] private float punchDuration = 0.25f;
    [SerializeField] private int vibrato = 8;
    [SerializeField] private float elasticity = 0.6f;
    [SerializeField] private AudioClip clickSound;

    private AudioSource _audioSource;
    private Tween _currentTween;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void ClickFeedback()
    {
        // stop any previous tween on this button
        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill();

        if (transform == null) return;

        _currentTween = transform.DOPunchScale(
            Vector3.one * punchStrength,
            punchDuration,
            vibrato,
            elasticity
        ).SetUpdate(true);

        if (_audioSource != null && clickSound != null)
            _audioSource.PlayOneShot(clickSound);
    }

    void OnDestroy()
    {
        // kill tween if button gets destroyed during scene reload
        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill();
    }
}