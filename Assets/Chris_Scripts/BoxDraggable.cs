using UnityEngine;
using System.Collections;

public class BoxDraggable : MonoBehaviour
{
    public int boxType;
    public ParticleSystem dragParticles;

    public bool isBeingHeld = false;

    // Add one color per pit in the Inspector — Unity will show a resizable array
    public Color[] pitColors;

    private Renderer boxRenderer;
    private PitTarget currentOverlappingPit = null;
    private Vector3 spawnPosition;
    private bool isFlashing = false;

    void Awake()
    {
        boxRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        spawnPosition = transform.position;
    }

    // Called by spawner after setting boxType
    public void UpdateVisual()
    {
        if (pitColors != null && boxType < pitColors.Length)
            boxRenderer.material.color = pitColors[boxType];
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

    public void OnPickUp()
    {
        isBeingHeld = true;
        if (dragParticles != null) dragParticles.Play();
    }

    public void OnRelease()
    {
        isBeingHeld = false;
        if (dragParticles != null) dragParticles.Stop();

        if (currentOverlappingPit != null)
        {
            if (currentOverlappingPit.pitType == boxType)
                GameManager.Instance.CorrectPit(gameObject);
            else
                GameManager.Instance.WrongPit();
        }
        else
        {
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

}