using UnityEngine;
using System.Collections;

public class BoxDraggable : MonoBehaviour
{
    public int boxType;
    public Color[] pitColors;

    public bool isBeingHeld = false;

    private Renderer boxRenderer;
    private PitTarget currentOverlappingPit = null;
    private Vector3 spawnPosition;
    private bool isFlashing = false;

    void Awake()
    {
        boxRenderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        // Reset state every time box is pulled from the pool
        isBeingHeld = false;
        isFlashing = false;
        currentOverlappingPit = null;
        spawnPosition = transform.position;
    }

    public void UpdateVisual()
    {
        if (pitColors != null && boxType < pitColors.Length)
            boxRenderer.material.color = pitColors[boxType];
    }

    public void OnPickUp()
    {
        isBeingHeld = true;
    }

    public void OnRelease()
    {
        isBeingHeld = false;

        if (currentOverlappingPit != null)
        {
            if (currentOverlappingPit.pitType == boxType)
                GameManager.Instance.CorrectPit(gameObject);
            else
                GameManager.Instance.WrongPit();
        }
        else
        {
            // Dropped in empty space — snap back to belt position
            transform.position = spawnPosition;
        }
    }

    public void FlashWarning()
    {
        if (!isFlashing) StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        isFlashing = true;
        Color originalColor = boxRenderer.material.color;

        for (int i = 0; i < 3; i++)
        {
            boxRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            boxRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        isFlashing = false;
    }

    void OnTriggerEnter(Collider other)
    {
        PitTarget pit = other.GetComponent<PitTarget>();
        if (pit != null) currentOverlappingPit = pit;
    }

    void OnTriggerExit(Collider other)
    {
        PitTarget pit = other.GetComponent<PitTarget>();
        if (pit == currentOverlappingPit) currentOverlappingPit = null;
    }
}