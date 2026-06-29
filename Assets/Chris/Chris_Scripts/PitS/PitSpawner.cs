using UnityEngine;
using System.Collections;

public class PitSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Overrides LevelData activation time for this specific pit. Set to -1 to use LevelData value.")]
    public float activationTimeOverride = -1f;

    [Tooltip("Overrides LevelData min interval for this pit. Set to -1 to use LevelData value.")]
    public float minIntervalOverride = -1f;

    [Tooltip("Overrides LevelData max interval for this pit. Set to -1 to use LevelData value.")]
    public float maxIntervalOverride = -1f;

    [Header("Which soul type this pit spawns")]
    public BoxType spawnedSoulType;

    [Header("Movement target")]
    [Tooltip("Leave empty to auto-find the GateTarget in the scene")]
    public Transform customTarget;

    private float spawnTimer;
    private bool isActive = false;
    private bool isStopped = false;
    private GateTarget gate;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        gate = FindObjectOfType<GateTarget>();
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive || isStopped) return;

        // Check activation time
        if (!isActive)
        {
            float activationTime = activationTimeOverride >= 0f
                ? activationTimeOverride
                : GameManager.Instance.currentLevel.pitSpawnActivationTime;

            if (GameManager.Instance.timeElapsed >= activationTime)
            {
                isActive = true;
                
            }
            return; // not active yet, don't count down spawn timer
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnSoul();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        LevelData lvl = GameManager.Instance.currentLevel;

        float min = minIntervalOverride >= 0f ? minIntervalOverride : lvl.pitMinSpawnInterval;
        float max = maxIntervalOverride >= 0f ? maxIntervalOverride : lvl.pitMaxSpawnInterval;

        spawnTimer = Random.Range(min, max);
    }

    void SpawnSoul()
    {
        // Spawn from this pit's position, slightly above so it doesn't clip the pit geometry
        Vector3 spawnPos = transform.position + Vector3.up * 1f;

        GameObject soul = BoxPool.Instance.GetBox(spawnedSoulType, spawnPos);
        if (soul == null)
        {
            
            return;
        }

        BoxDraggable draggable = soul.GetComponent<BoxDraggable>();

        // Notify the spawner system a new soul is active
        BoxSpawner.Instance.RegisterExternalSoul();

        // Start moving toward the gate
        StartCoroutine(MoveTowardGate(soul));

        
    }

    IEnumerator MoveTowardGate(GameObject soul)
    {
        Transform target = customTarget != null
            ? customTarget
            : gate != null ? gate.transform : null;

        if (target == null)
        {
            
            yield break;
        }

        while (soul != null && soul.activeSelf)
        {
            BoxDraggable draggable = soul.GetComponent<BoxDraggable>();

            if (!draggable.isBeingHeld)
            {
                // Use the urgency-adjusted belt speed from BoxSpawner
                float speed = BoxSpawner.Instance.GetCurrentBeltSpeed();

                soul.transform.position = Vector3.MoveTowards(
                    soul.transform.position,
                    target.position,
                    speed * Time.deltaTime
                );

                float distanceLeft = Vector3.Distance(soul.transform.position, target.position);

                if (distanceLeft < 1.5f)
                    draggable.FlashWarning();
            }

            yield return null;
        }
    }

    public void StopSpawning()
    {
        isStopped = true;
        StopAllCoroutines();
    }
}