using TMPro;
using UnityEngine;

public class RoundUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roundText;

    void Start()
    {
        UpdateRound();
    }

    void UpdateRound()
    {
        int round = GameManager.GetCurrentRound();
        _roundText.text = "ROUND " + round;
    }
}