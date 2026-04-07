using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _defaultDuration = 0.15f;
    [SerializeField] private float _defaultStrength = 0.15f;

    private Vector3 _shakeStartPosition;
    private Coroutine _shakeCoroutine;

    public void Shake()
    {
        Shake(_defaultDuration, _defaultStrength);
    }

    public void Shake(float duration, float strength)
    {
        // save the camera position at the exact moment the shake starts
        _shakeStartPosition = transform.position;

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            transform.position = _shakeStartPosition;
        }

        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * strength;

            transform.position = _shakeStartPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _shakeStartPosition;
        _shakeCoroutine = null;
    }

    private void OnDisable()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }

        transform.position = _shakeStartPosition;
    }
}