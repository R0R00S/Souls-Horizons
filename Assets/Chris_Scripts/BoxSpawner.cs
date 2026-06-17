using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    public static BoxSpawner Instance;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform beltEndPoint;

    // CHANGED: replaced single currentBox with a counter
    private int activeBoxCount = 0;
    private int maxSimultaneousBoxes = 1; // start at 1, raise later via LevelData

    private float spawnTimer;
    private bool isStopped = false;

    // NEW: tracked separately so the coroutine can read it
    private float currentBeltSpeed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnTimer = GameManager.Instance.currentLevel.spawnInterval;
        currentBeltSpeed = GameManager.Instance.currentLevel.beltSpeed;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive || isStopped) return;

        // NEW: difficulty ramp — belt speeds up in the last 20 seconds
        float urgencyMultiplier = GameManager.Instance.timeRemaining < 20f ? 1.5f : 1f;
        currentBeltSpeed = GameManager.Instance.currentLevel.beltSpeed * urgencyMultiplier;

        // CHANGED: spawn if below the simultaneous box limit
        if (activeBoxCount < maxSimultaneousBoxes)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnBox();
                spawnTimer = GameManager.Instance.currentLevel.spawnInterval;
            }
        }
    }

    void SpawnBox()
    {
        // NEW: stagger spawn positions so multiple boxes don't overlap
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x += activeBoxCount * 1.2f;

        GameObject box = BoxPool.Instance.GetBox(spawnPos);
        if (box == null)
        {
            Debug.LogWarning("Box pool exhausted — increase pool size");
            return;
        }

        int randomType = Random.Range(0, GameManager.Instance.currentLevel.numberOfPits);
        BoxDraggable draggable = box.GetComponent<BoxDraggable>();
        draggable.boxType = randomType;
        draggable.UpdateVisual();

        // CHANGED: increment counter instead of storing reference
        activeBoxCount++;

        StartCoroutine(MoveAlongBelt(box));

        Debug.Log("Spawned box of type: " + randomType);
    }

    IEnumerator MoveAlongBelt(GameObject box)
    {
        while (box != null && box.activeSelf)
        {
            BoxDraggable draggable = box.GetComponent<BoxDraggable>();

            if (!draggable.isBeingHeld)
            {
                box.transform.position = Vector3.MoveTowards(
                    box.transform.position,
                    beltEndPoint.position,
                    currentBeltSpeed * Time.deltaTime // NEW: uses the live speed value
                );

                // NEW: flash warning when close to falling off
                float distanceLeft = Vector3.Distance(
                    box.transform.position,
                    beltEndPoint.position
                );
                if (distanceLeft < 1.5f)
                    draggable.FlashWarning();

                // Box reached the end — return it to pool
                if (distanceLeft < 0.1f)
                {
                    ReturnBoxToBelt(box);
                    yield break;
                }
            }

            yield return null;
        }
    }

    public void ReturnBoxToBelt(GameObject box)
    {
        BoxPool.Instance.ReturnBox(box);
        // CHANGED: decrement counter instead of clearing reference
        activeBoxCount = Mathf.Max(0, activeBoxCount - 1);
        Debug.Log("Box returned to pool. Active boxes: " + activeBoxCount);
    }

    // CHANGED: now decrements counter instead of clearing reference
    public void BoxSorted()
    {
        activeBoxCount = Mathf.Max(0, activeBoxCount - 1);
    }

    public void StopSpawning()
    {
        isStopped = true;
        StopAllCoroutines();
        activeBoxCount = 0;
    }
}