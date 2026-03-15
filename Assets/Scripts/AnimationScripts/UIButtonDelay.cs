using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class UIButtonActionDelay : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float punchStrength = 0.25f;
    [SerializeField] private float punchDuration = 0.4f;
    [SerializeField] private int vibrato = 8;
    [SerializeField] private float elasticity = 0.6f;

    [Header("Extra Delay")]
    // Extra wait after the animation finishes
    [SerializeField] private float extraDelay = 0f;

    [Header("Sound")]
    [SerializeField] private AudioClip clickSound;

    [Header("Action After Animation")]
    [SerializeField] private UnityEvent action;

    private AudioSource _audio;
    private bool _clicked = false;
    private Tween _tween;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (_clicked) return;
        _clicked = true;

        // Play click sound
        if (_audio != null && clickSound != null)
            _audio.PlayOneShot(clickSound);

        // Kill old tween if there is one
        if (_tween != null && _tween.IsActive())
            _tween.Kill();

        // Play the punch animation
        _tween = transform.DOPunchScale(
            Vector3.one * punchStrength,
            punchDuration,
            vibrato,
            elasticity
        ).SetUpdate(true);

        // Wait manually, then do the action
        StartCoroutine(PlayThenInvoke());
    }

    IEnumerator PlayThenInvoke()
    {
        // Wait for animation time + extra delay
        yield return new WaitForSecondsRealtime(punchDuration + extraDelay);

        action?.Invoke();
    }

    void OnDestroy()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();
    }
}