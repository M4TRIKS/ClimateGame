using UnityEngine;

[System.Serializable]
public class FactoryLevelData
{
    public int baseProduction = 1;
    public float cooldown = 5f;
    public float comboMultiplier = 2f;
    public Sprite sprite;
}

[CreateAssetMenu(menuName = "Factory/Factory Data")]
public class FactoryData : ScriptableObject
{
    [Header("Combo")]
    public Vector2Int[] comboPattern;

    [Header("Levels")]
    public FactoryLevelData[] levels;
}