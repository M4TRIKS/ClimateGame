using UnityEngine;

[System.Serializable]

public class FactoryLevelData
{
    public int baseProduction = 1;
    public float cooldown = 5f;
    public float comboMultiplier = 2f;

    [Header("Visuals")]
    public Sprite sprite; // if no animation frames are assigned 
    public Sprite[] animationFrames;
    public float animationFrameRate = 0.2f;
}
[CreateAssetMenu(menuName = "Factory/Factory Data")]
public class FactoryData : ScriptableObject
{
    [Header("Combo")]
    public Vector2Int[] comboPattern;

    [Header("Levels")]
    public FactoryLevelData[] levels;
}