using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int numberOfPits = 2;
    public float levelDuration = 60f;
    public float spawnInterval = 3f;
    public float beltSpeed = 2f;
    public int maxSimultaneousBoxes = 1;
}