using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance;

    [SerializeField] Image flashOverlay;
    [SerializeField] Color flashColor = new Color(0f, 0f, 0f, 0.6f); // dark, semi-transparent
    [SerializeField] float flashDuration = 0.15f;

    void Awake()
    {
        Instance = this;
    }

    public void Flash()
    {
        StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        // Fade in
        float halfDuration = flashDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            float alpha = Mathf.Lerp(0f, flashColor.a, elapsed / halfDuration);
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float alpha = Mathf.Lerp(flashColor.a, 0f, elapsed / halfDuration);
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Make sure it ends fully transparent
        flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }
}