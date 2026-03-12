using UnityEngine;
using TMPro;

public class ResourcePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro _textMesh;

    [Header("Motion")]
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _lifetime = 1f;
    [SerializeField] private float _fadeSpeed = 3f;

    [Header("Color Gradient")]
    [SerializeField] private Gradient _colorGradient;

    [Header("Production Range")]
    [SerializeField] private float _minProduction = 1f;
    [SerializeField] private float _maxProduction = 10f;

    private float _timer;
    private Color _currentColor;

    public static ResourcePopup Create(ResourcePopup prefab, Vector3 position, int amount)
    {
        ResourcePopup popup = Instantiate(prefab, position, Quaternion.identity);
        popup.Setup(amount);
        return popup;
    }

    private void Awake()
    {
        if (_textMesh == null)
            _textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int amount)
    {
        _textMesh.SetText(amount.ToString());

        float t = Mathf.InverseLerp(_minProduction, _maxProduction, amount);
        _currentColor = _colorGradient.Evaluate(t);

        _textMesh.color = _currentColor;

        _timer = _lifetime;
    }

    private void Update()
    {
        transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _currentColor.a -= _fadeSpeed * Time.deltaTime;
            _textMesh.color = _currentColor;

            if (_currentColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}