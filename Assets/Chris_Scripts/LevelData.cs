using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public float levelDuration = 60f;
    public float beltSpeed = 2f;
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 4f;
    public int maxSimultaneousBoxes = 1;

    [Header("Pit Settings")]
    public int numberOfPits = 2;

    [Header("Spawn Weights — higher = more frequent")]
    [Range(0, 100)] public int noneChance = 20; // safe boxes that should enter the gate
    [Range(0, 100)] public int typeAChance = 40;
    [Range(0, 100)] public int typeBChance = 40;
    [Range(0, 100)] public int typeCChance = 0;
    [Range(0, 100)] public int typeDChance = 0;
}