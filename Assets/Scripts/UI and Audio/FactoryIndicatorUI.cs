using UnityEngine;

public class FactoryIndicatorUI : MonoBehaviour
{
    [Header("Arrow Prefabs")]
    [SerializeField] private GameObject _upgradeArrowPrefab;
    [SerializeField] private GameObject _pollutionArrowPrefab;

    [Header("Arrow Positions")]
    [SerializeField] private Vector3 _upgradeArrowOffset = new Vector3(0.35f, 0.35f, 0f);
    [SerializeField] private Vector3 _pollutionArrowOffset = new Vector3(0.35f, -0.35f, 0f);

    private GameObject _upgradeArrowInstance;
    private GameObject _pollutionArrowInstance;

    public void ShowUpgradeArrow()
    {
        if (_upgradeArrowPrefab == null) return;

        if (_upgradeArrowInstance == null)
        {
            _upgradeArrowInstance = Instantiate(
                _upgradeArrowPrefab,
                transform.position + _upgradeArrowOffset,
                Quaternion.identity,
                transform
            );
        }

        _upgradeArrowInstance.SetActive(true);
    }

    public void ShowPollutionArrow()
    {
        if (_pollutionArrowPrefab == null) return;

        if (_pollutionArrowInstance == null)
        {
            _pollutionArrowInstance = Instantiate(
                _pollutionArrowPrefab,
                transform.position + _pollutionArrowOffset,
                Quaternion.identity,
                transform
            );
        }

        _pollutionArrowInstance.SetActive(true);
    }
}