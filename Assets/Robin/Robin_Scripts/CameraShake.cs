using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPosition;

    void Awake()
    {
        Instance = this;
        originalPosition = transform.localPosition;
    }

    public void Shake(float duration = 0.3f, float magnitude = 0.15f)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Random offset that decreases over time (fades out toward end)
            float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration);

            transform.localPosition = originalPosition + new Vector3(
                Random.Range(-1f, 1f) * strength,
                Random.Range(-1f, 1f) * strength,
                0f  // don't shake Z so camera doesn't drift toward/away from scene
            );

            elapsed += Time.unscaledDeltaTime; // unscaled so it works even if Time.timeScale = 0
            yield return null;
        }

        // Always snap back to exact original position when done
        transform.localPosition = originalPosition;
    }
}