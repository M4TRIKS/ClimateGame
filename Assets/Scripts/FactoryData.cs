using UnityEngine;

[CreateAssetMenu(menuName = "Factory/Factory Data")]
public class FactoryData : ScriptableObject
{
    [Header("Production")]
    public int baseProduction = 1;
    public float cooldown = 5f;

    [Header("Combo")]
    public Vector2Int[] comboPattern;   // tiles required relative to center
    public float comboMultiplier = 2f;

    [Header("Visual")]
    public Sprite sprite;
}