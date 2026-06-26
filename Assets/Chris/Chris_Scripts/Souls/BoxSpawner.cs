using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    public static BoxSpawner Instance;

    [Header("Belt")]
    public Transform spawnPoint;

    private int activeBoxCount = 0;
    private float spawnTimer;
    private float currentBeltSpeed;
    private bool isStopped = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnTimer = GameManager.Instance.currentLevel.minSpawnInterval;
        currentBeltSpeed = GameManager.Instance.currentLevel.beltSpeed;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive || isStopped) return;

        
       
        LevelData lvl = GameManager.Instance.currentLevel;
        float urgencyMultiplier = GameManager.Instance.timeRemaining < lvl.urgencyTimeThreshold
            ? lvl.urgencySpeedMultiplier
            : 1f;
        currentBeltSpeed = lvl.beltSpeed * urgencyMultiplier;

        currentBeltSpeed = Mathf.Min(
        lvl.beltSpeed * urgencyMultiplier,
        lvl.maxBeltSpeed
        );

        if (activeBoxCount < GameManager.Instance.currentLevel.maxSimultaneousBoxes)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnBox();
                spawnTimer = Random.Range(
                    GameManager.Instance.currentLevel.minSpawnInterval,
                    GameManager.Instance.currentLevel.maxSpawnInterval
                );
            }
        }
    }

    void SpawnBox()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x += Random.Range(-0.3f, 0.3f);

        BoxType chosenType = PickBoxType();

        GameObject box = BoxPool.Instance.GetBox(chosenType, spawnPos);
        if (box == null) return;

        activeBoxCount++;
        StartCoroutine(MoveAlongBelt(box));

        Debug.Log("Spawned box type: " + chosenType);
    }

    BoxType PickBoxType()
    {
        LevelData lvl = GameManager.Instance.currentLevel;

        int total = lvl.noneChance + lvl.typeAChance + lvl.typeBChance
                  + lvl.typeCChance + lvl.typeDChance;

        if (total == 0) return BoxType.None;

        int roll = Random.Range(0, total);

        if (roll < lvl.noneChance) return BoxType.None;
        roll -= lvl.noneChance;

        if (roll < lvl.typeAChance) return BoxType.TypeA;
        roll -= lvl.typeAChance;

        if (roll < lvl.typeBChance) return BoxType.TypeB;
        roll -= lvl.typeBChance;

        if (roll < lvl.typeCChance) return BoxType.TypeC;

        return BoxType.TypeD;
    }

    IEnumerator MoveAlongBelt(GameObject box)
    {
        GateTarget gate = FindObjectOfType<GateTarget>();

        while (box != null && box.activeSelf)
        {
            BoxDraggable draggable = box.GetComponent<BoxDraggable>();

            if (!draggable.isBeingHeld && gate != null)
            {
                box.transform.position = Vector3.MoveTowards(
                    box.transform.position,
                    gate.transform.position,
                    currentBeltSpeed * Time.deltaTime
                );

                float distanceLeft = Vector3.Distance(
                    box.transform.position,
                    gate.transform.position
                );

                if (distanceLeft < 1.5f)
                    draggable.FlashWarning();
            }

            yield return null;
        }
    }

    public void ReturnBoxToBelt(GameObject box)
    {
        BoxPool.Instance.ReturnBox(box);
        activeBoxCount = Mathf.Max(0, activeBoxCount - 1);
    }

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

    // Lets PitSpawner register souls it spawned into the shared active count
    public void RegisterExternalSoul()
    {
        activeBoxCount++;
    }

    // Lets PitSpawner read the live belt speed including urgency multiplier
    public float GetCurrentBeltSpeed()
    {
        return currentBeltSpeed;
    }
}