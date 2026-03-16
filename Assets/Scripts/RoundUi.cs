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
        int round = GameManager.GetCurrentRound(); // get round from GameManager
        _roundText.text = "ROUND " + round;
    }
}