using UnityEngine;

public class FlameController : MonoBehaviour
{
    [Header("Visual")]
    public Transform spriteVisual;

    // Size-to-scale conversion — adjust this multiplier to match your sprite's natural size
    public float visualScaleMultiplier = 0.15f;

    private float explodeSize;
    private float extinguishSize;
    private float growthRate;
    private float shrinkRate;

    private float currentSize;
    private bool isBeingExtinguished = false;
    private bool isExploded = false;

    public void Initialize(float startSize, float explodeAt, float extinguishAt,
                            float growRate, float shrinkRatePerSec)
    {
        currentSize = startSize;
        explodeSize = explodeAt;
        extinguishSize = extinguishAt;
        growthRate = growRate;
        shrinkRate = shrinkRatePerSec;

        isBeingExtinguished = false;
        isExploded = false;
        UpdateVisualScale();
    }

    void Update()
    {
        if (isExploded) return;

        if (isBeingExtinguished)
        {
            currentSize -= shrinkRate * Time.deltaTime;

            if (currentSize <= extinguishSize)
            {
                Extinguish();
                return;
            }
        }
        else
        {
            currentSize += growthRate * Time.deltaTime;

            if (currentSize >= explodeSize)
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
        {
            float scale = currentSize * visualScaleMultiplier;
            spriteVisual.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void StartExtinguishing()
    {
        isBeingExtinguished = true;
    }

    public void StopExtinguishing()
    {
        isBeingExtinguished = false;
        // No reset needed — size just stays where it is and resumes growing from there
    }

    void Extinguish()
    {
        Debug.Log("Flame extinguished at size " + currentSize);
        FlameSpawner.Instance.FlameRemoved(gameObject);
    }

    void Explode()
    {
        isExploded = true;
        Debug.Log("Flame exploded at size " + currentSize);
        GameManager.Instance.WrongPit();
        FlameSpawner.Instance.FlameRemoved(gameObject);
    }
}