using UnityEngine;

public class FlameController : MonoBehaviour
{
    [Header("Visual")]
    public Transform spriteVisual; // the 2D flame sprite child object
    public Vector3 minScale = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);

    private float growthDuration;
    private float extinguishHoldTime;
    private float currentGrowth = 0f;   // 0 = just spawned, 1 = about to explode
    private float extinguishProgress = 0f; // 0 = untouched, 1 = fully extinguished
    private bool isBeingExtinguished = false;
    private bool isExploded = false;

    public void Initialize(float growDuration, float extinguishTime)
    {
        growthDuration = growDuration;
        extinguishHoldTime = extinguishTime;
        currentGrowth = 0f;
        extinguishProgress = 0f;
        isBeingExtinguished = false;
        isExploded = false;
        UpdateVisualScale();
    }

    void Update()
    {
        if (isExploded) return;

        if (isBeingExtinguished)
        {
            // Holding press — shrink back down
            extinguishProgress += Time.deltaTime / extinguishHoldTime;
            currentGrowth = Mathf.Max(0f, currentGrowth - (Time.deltaTime / extinguishHoldTime));

            if (extinguishProgress >= 1f)
            {
                Extinguish();
                return;
            }
        }
        else
        {
            // Left alone — keep growing
            currentGrowth += Time.deltaTime / growthDuration;

            if (currentGrowth >= 1f)
            {
                Explode();
                return;
            }
        }

        UpdateVisualScale();
    }

    void UpdateVisualScale()
    {
        if (spriteVisual != null)
            spriteVisual.localScale = Vector3.Lerp(minScale, maxScale, currentGrowth);
    }

    // Called by FlameInputHandler while the player holds their finger on this flame
    public void StartExtinguishing()
    {
        isBeingExtinguished = true;
    }

    // Called when the player lifts their finger before fully extinguishing
    public void StopExtinguishing()
    {
        isBeingExtinguished = false;
        // Alternative — decays gradually instead of resetting to 0
        extinguishProgress = Mathf.Max(0f, extinguishProgress - Time.deltaTime * 0.5f);
    }

    void Extinguish()
    {
        Debug.Log("Flame extinguished");
        FlameSpawner.Instance.FlameRemoved(gameObject);
    }

    void Explode()
    {
        isExploded = true;
        Debug.Log("Flame exploded! Lose a life.");
        GameManager.Instance.WrongPit(); // reuses existing life-loss logic
        FlameSpawner.Instance.FlameRemoved(gameObject);
    }
}