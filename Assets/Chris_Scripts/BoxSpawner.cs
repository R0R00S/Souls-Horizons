using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    public static BoxSpawner Instance;

    [Header("Belt")]
    public Transform spawnPoint;
    public Transform beltEndPoint;

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
        spawnTimer = GameManager.Instance.currentLevel.spawnInterval;
        currentBeltSpeed = GameManager.Instance.currentLevel.beltSpeed;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive || isStopped) return;

        // Belt speeds up in the last 20 seconds
        float urgencyMultiplier = GameManager.Instance.timeRemaining < 20f ? 1.5f : 1f;
        currentBeltSpeed = GameManager.Instance.currentLevel.beltSpeed * urgencyMultiplier;

        if (activeBoxCount < GameManager.Instance.currentLevel.maxSimultaneousBoxes)
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
        // Stagger positions so multiple boxes don't overlap
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x += activeBoxCount * 1.2f;

        GameObject box = BoxPool.Instance.GetBox(spawnPos);
        if (box == null) return;

        int randomType = Random.Range(0, GameManager.Instance.currentLevel.numberOfPits);
        BoxDraggable draggable = box.GetComponent<BoxDraggable>();
        draggable.boxType = randomType;
        draggable.UpdateVisual();

        activeBoxCount++;
        StartCoroutine(MoveAlongBelt(box));

        Debug.Log("Spawned box type: " + randomType);
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
                    currentBeltSpeed * Time.deltaTime
                );

                float distanceLeft = Vector3.Distance(
                    box.transform.position,
                    beltEndPoint.position
                );

                if (distanceLeft < 1.5f)
                    draggable.FlashWarning();

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
}