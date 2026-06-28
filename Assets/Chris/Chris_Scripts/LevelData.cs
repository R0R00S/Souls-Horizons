using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Scene")]
    public string sceneName = "Game_Level1"; // must match the scene filename exactly


    [Header("Level Settings")]
    public float levelDuration = 60f;
    public float beltSpeed = 2f;
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 4f;
    public int maxSimultaneousBoxes = 1;
    public float maxBeltSpeed = 4f;

    [Header("Pit Settings")]
    public int numberOfPits = 2;

    [Header("Spawn Weights — higher = more frequent")]
    [Range(0, 100)] public int noneChance = 20; // safe boxes that should enter the gate
    [Range(0, 100)] public int typeAChance = 40;
    [Range(0, 100)] public int typeBChance = 40;
    [Range(0, 100)] public int typeCChance = 0;
    [Range(0, 100)] public int typeDChance = 0;

    [Header("Time Cap")]
    [Tooltip("Maximum seconds the timer can reach including penalties. Set to 0 for no cap.")]
    public float maxTimeCap = 0f; // 0 means uncapped by default

    [Header("Scoring")]
    [Tooltip("Starting score before any penalties")]
    public int baseScore = 1000;

    [Tooltip("Score lost per extra second added to the timer beyond the starting duration")]
    public int scoreLostPerPenaltySecond = 5;

    [Tooltip("Score lost per life lost. Index 0 = first life lost, index 1 = second life lost")]
    public int[] scoreLostPerLife = { 100, 300 };

    [Header("Difficulty Ramp")]
    [Tooltip("Urgency speed boost kicks in when time remaining drops below this value")]
    public float urgencyTimeThreshold = 20f;
    [Tooltip("Speed multiplier applied during urgency phase")]
    public float urgencySpeedMultiplier = 1.5f;



    [Header("Pit Spawner Settings")]
    public float pitSpawnActivationTime = 30f;  // seconds into the level before pit spawning begins
    public float pitMinSpawnInterval = 5f;       // slower than the belt by default
    public float pitMaxSpawnInterval = 12f;

    [Header("Dialogue")]
    public LevelDialogueData dialogueData; // optional — leave empty for no dialogue

    [Header("Flame Penalty")]
    [Tooltip("Seconds added to the timer when a soul touches a flame")]
    public float flamePenaltySeconds = 10f;

    void OnValidate()
    {
        if (urgencyTimeThreshold >= levelDuration)
        {
            urgencyTimeThreshold = levelDuration * 0.25f;
            Debug.LogWarning(name + ": urgencyTimeThreshold was longer than the level, auto-corrected.");
        }

        if (pitSpawnActivationTime >= levelDuration)
            Debug.LogWarning(name + ": pitSpawnActivationTime is after level ends — pits will never spawn.");
    }

}