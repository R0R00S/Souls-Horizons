using UnityEngine;

public class ElysiumGate : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Soul soul = other.GetComponent<Soul>();

        if (soul == null) return;
        if (soul.isHeld) return; // ignore souls being dragged

        if (soul.sinType != SinType.None)
        {
            Debug.Log($"A sinful soul ({soul.sinType}) reached Elysium! Lose a life.");
            // We'll hook this into GameManager next
        }
        else
        {
            Debug.Log("Innocent soul reached Elysium safely.");
        }

        Destroy(soul.gameObject);
    }
}