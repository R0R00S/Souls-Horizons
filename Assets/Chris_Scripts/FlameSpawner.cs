using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlameSpawner : MonoBehaviour
{
    public static FlameSpawner Instance;

    [Header("Setup")]
    public GameObject flamePrefab;
    public Transform[] noSpawnZones; // drag belt, pits, and gate transforms here
    public float noSpawnRadius = 1.5f; // how close a flame can't spawn to those zones

    [Header("Map bounds for random spawn position")]
    public Vector2 mapMinXZ = new Vector2(-5f, -3f);
    public Vector2 mapMaxXZ = new Vector2(5f, 3f);
    public float spawnHeight = 0.1f;

    private List<GameObject> activeFlames = new List<GameObject>();
    private float spawnTimer;
    private bool isStopped = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive || isStopped) return;

        if (activeFlames.Count < GameManager.Instance.currentLevel.maxActiveFlames)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                TrySpawnFlame();
                ScheduleNextSpawn();
            }
        }
    }

    void ScheduleNextSpawn()
    {
        LevelData lvl = GameManager.Instance.currentLevel;
        spawnTimer = Random.Range(lvl.flameMinSpawnInterval, lvl.flameMaxSpawnInterval);
    }

    void TrySpawnFlame()
    {
        Vector3 candidate = GetValidSpawnPosition();
        if (candidate == Vector3.zero) return; // failed to find a spot, skip this cycle

        GameObject flame = Instantiate(flamePrefab, candidate, Quaternion.identity);
        FlameController controller = flame.GetComponent<FlameController>();

        LevelData lvl = GameManager.Instance.currentLevel;
        controller.Initialize(lvl.flameGrowthDuration, lvl.flameExtinguishHoldTime);

        activeFlames.Add(flame);
        Debug.Log("Flame spawned at " + candidate);
    }

    Vector3 GetValidSpawnPosition()
    {
        int maxAttempts = 10; // avoid infinite loop if map is crowded

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float x = Random.Range(mapMinXZ.x, mapMaxXZ.x);
            float z = Random.Range(mapMinXZ.y, mapMaxXZ.y);
            Vector3 candidate = new Vector3(x, spawnHeight, z);

            if (IsFarEnoughFromZones(candidate))
                return candidate;
        }

        return Vector3.zero; // no valid spot found this cycle
    }

    bool IsFarEnoughFromZones(Vector3 position)
    {
        foreach (Transform zone in noSpawnZones)
        {
            if (Vector3.Distance(position, zone.position) < noSpawnRadius)
                return false;
        }
        return true;
    }

    // Called by FlameController when extinguished or exploded
    public void FlameRemoved(GameObject flame)
    {
        activeFlames.Remove(flame);
        Destroy(flame);
    }

    public void StopSpawning()
    {
        isStopped = true;

        // Clean up any remaining flames when game ends
        foreach (var flame in activeFlames)
            if (flame != null) Destroy(flame);
        activeFlames.Clear();
    }
}