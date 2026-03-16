using UnityEngine;

[System.Serializable]
public class FactoryLevelData
{
    public int baseProduction = 1;      // how much it produces
    public float cooldown = 5f;         // time between productions
    public float comboMultiplier = 2f;  // multiplier when combo is active

    [Header("Visuals")]
    public Sprite sprite; // if no animation frames are assigned 
    public Sprite[] animationFrames;
    public float animationFrameRate = 0.2f;
}

[CreateAssetMenu(menuName = "Factory/Factory Data")]
public class FactoryData : ScriptableObject
{
    [Header("Combo")]
    public Vector2Int[] comboPattern; // pattern needed for combo

    [Header("Levels")]
    public FactoryLevelData[] levels; // upgrade levels
}