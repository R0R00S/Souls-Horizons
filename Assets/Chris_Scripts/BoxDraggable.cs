using UnityEngine;

public class BoxDraggable : MonoBehaviour
{
    public int boxType; // 0 = goes in pit A, 1 = goes in pit B
    public ParticleSystem dragParticles;

    private Vector3 spawnPosition; // so we can snap it back if dropped nowhere

    void Start()
    {
        spawnPosition = transform.position;
    }

    public void OnPickUp()
    {
        if (dragParticles != null) dragParticles.Play();
    }

    public void OnRelease()
    {
        if (dragParticles != null) dragParticles.Stop();

        // Check if we're overlapping a pit
        Collider[] nearby = Physics.OverlapSphere(transform.position, 1f);
        PitTarget pitHit = null;

        foreach (var col in nearby)
        {
            PitTarget pit = col.GetComponent<PitTarget>();
            if (pit != null) { pitHit = pit; break; }
        }

        if (pitHit != null)
        {
            if (pitHit.pitType == boxType)
                GameManager.Instance.CorrectPit(gameObject);
            else
                GameManager.Instance.WrongPit();
            // Optional: snap back to belt after wrong pit
        }
        else
        {
            // Dropped in empty space — snap back
            transform.position = spawnPosition;
            Debug.Log("Dropped in empty space, snapping back");
        }
    }
}