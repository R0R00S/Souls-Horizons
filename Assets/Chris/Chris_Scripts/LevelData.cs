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

    [Header("Pit Spawner Settings")]
    [Tooltip("Pit spawning activates when the timer reaches this many seconds remaining")]
    public float pitSpawnActivationTimeRemaining = 30f;
    public float pitMinSpawnInterval = 5f;       // slower than the belt by default
    public float pitMaxSpawnInterval = 12f;
    
    
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




    [Header("Dialogue")]
    public LevelDialogueData dialogueData; // optional — leave empty for no dialogue

    [Header("Flame Penalty")]
    [Tooltip("Seconds added to the timer when a soul touches a flame")]
    public float flamePenaltySeconds = 10f;

    [Header("Time Soul Settings")]
    public bool enableTimeSoul = true;
    [Tooltip("Minimum seconds elapsed before first time soul can spawn")]
    public float timeSoulMinSpawnWindow = 15f;
    [Tooltip("Maximum seconds elapsed before first time soul must spawn")]
    public float timeSoulMaxSpawnWindow = 35f;
    [Tooltip("How much faster the time soul moves compared to regular belt speed")]
    public float timeSoulSpeedMultiplier = 2.2f;
    [Tooltip("Seconds added to timer if time soul reaches the gate")]
    public float timeSoulPenaltySeconds = 20f;
    [Tooltip("Seconds removed from timer if player sorts time soul correctly")]
    public float timeSoulRewardSeconds = 10f;
    [Tooltip("After resolving a time soul, minimum seconds before another can spawn")]
    public float timeSoulCooldownMin = 20f;
    [Tooltip("After resolving a time soul, maximum seconds before another can spawn")]
    public float timeSoulCooldownMax = 40f;


    void OnValidate()
    {
        if (urgencyTimeThreshold >= levelDuration)
        {
            urgencyTimeThreshold = levelDuration * 0.25f;
            
        }

        if (pitSpawnActivationTimeRemaining >= levelDuration)
            Debug.LogWarning(name + ": pitSpawnActivationTimeRemaining is after level ends — pits will never spawn.");
    }

}