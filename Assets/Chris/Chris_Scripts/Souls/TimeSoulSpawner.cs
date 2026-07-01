using UnityEngine;
using System.Collections;

public class TimeSoulSpawner : MonoBehaviour
{
    public static TimeSoulSpawner Instance;

    [Header("Time Soul Prefabs — add one per type")]
    public GameObject[] timeSoulPrefabs; // drag your 3 prefabs here in Inspector

    private bool isStopped = false;
    private bool timeSoulCurrentlyActive = false;
    private float nextSpawnAtElapsed = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ScheduleNextSpawn(true);
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive || isStopped) return;
        if (!GameManager.Instance.currentLevel.enableTimeSoul) return; // add this
        if (timeSoulCurrentlyActive) return;

        if (GameManager.Instance.timeElapsed >= nextSpawnAtElapsed)
            SpawnTimeSoul();
    }

    void ScheduleNextSpawn(bool isFirstSpawn)
    {
        LevelData lvl = GameManager.Instance.currentLevel;

        float min = isFirstSpawn
            ? lvl.timeSoulMinSpawnWindow
            : GameManager.Instance.timeElapsed + lvl.timeSoulCooldownMin;

        float max = isFirstSpawn
            ? lvl.timeSoulMaxSpawnWindow
            : GameManager.Instance.timeElapsed + lvl.timeSoulCooldownMax;

        nextSpawnAtElapsed = Random.Range(min, max);
        Debug.Log("Next time soul scheduled at elapsed: " + nextSpawnAtElapsed);
    }

    void SpawnTimeSoul()
    {
        if (timeSoulPrefabs == null || timeSoulPrefabs.Length == 0)
        {
            Debug.LogWarning("No time soul prefabs assigned");
            return;
        }

        // Pick a random prefab from the array
        int randomIndex = Random.Range(0, timeSoulPrefabs.Length);
        GameObject chosenPrefab = timeSoulPrefabs[randomIndex];

        Vector3 spawnPos = BoxSpawner.Instance.GetSpawnPosition();
        GameObject soul = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

        timeSoulCurrentlyActive = true;
        BoxSpawner.Instance.RegisterExternalSoul();

        StartCoroutine(MoveTimeSoul(soul));

        Debug.Log("Time soul spawned of type: " +
                  soul.GetComponent<BoxDraggable>().boxType);
    }

    IEnumerator MoveTimeSoul(GameObject soul)
    {
        GateTarget gate = FindObjectOfType<GateTarget>();
        BoxDraggable draggable = soul.GetComponent<BoxDraggable>();

        LevelData lvl = GameManager.Instance.currentLevel;
        float speed = lvl.beltSpeed * lvl.timeSoulSpeedMultiplier;

        while (soul != null && soul.activeSelf)
        {
            if (!draggable.isBeingHeld && gate != null)
            {
                soul.transform.position = Vector3.MoveTowards(
                    soul.transform.position,
                    gate.transform.position,
                    speed * Time.deltaTime
                );
            }
            yield return null;
        }
    }

    public void TimeSoulResolved(GameObject soul)
    {
        timeSoulCurrentlyActive = false;
        Destroy(soul); // just destroy directly since time souls aren't pooled
        ScheduleNextSpawn(false);
    }

    public void StopSpawning()
    {
        isStopped = true;
        StopAllCoroutines();
    }
}