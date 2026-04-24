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

    // static create method so other scripts can spawn popup easily
    public static ResourcePopup Create(ResourcePopup prefab, Vector3 position, int amount)
    {
        ResourcePopup popup = Instantiate(prefab, position, Quaternion.identity);
        popup.Setup(amount);
        return popup;
    }

    private void Awake()
    {
        // auto find TMP if not assigned
        if (_textMesh == null)
            _textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int amount)
    {
        // show amount text
        _textMesh.SetText(amount.ToString());

        // color depends on production amount
        float t = Mathf.InverseLerp(_minProduction, _maxProduction, amount);
        _currentColor = _colorGradient.Evaluate(t);

        _textMesh.color = _currentColor;

        // set life timer
        _timer = _lifetime;
    }

    private void Update()
    {
        // move up every frame
        transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

        _timer -= Time.deltaTime;

        // start fading after life time ends
        if (_timer <= 0f)
        {
            _currentColor.a -= _fadeSpeed * Time.deltaTime;
            _textMesh.color = _currentColor;

            // destroy when fully invisible
            if (_currentColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}