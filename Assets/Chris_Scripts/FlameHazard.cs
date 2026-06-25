using UnityEngine;

public class FlameHazard : MonoBehaviour
{
    // Only hurts souls being actively dragged by the player
    void OnTriggerEnter(Collider other)
    {
        BoxDraggable box = other.GetComponent<BoxDraggable>();

        if (box == null) return;
        if (!box.isBeingHeld) return;

        Debug.Log("Dragged soul touched a flame — lose a life");

        // Force release first so InputManager doesn't hold a reference to a destroyed box
        InputManager.Instance.ForceRelease();

        // Then remove it the same way a wrong pit does
        GameManager.Instance.WrongPit();
        BoxSpawner.Instance.BoxSorted();
        BoxPool.Instance.ReturnBox(other.gameObject);
    }
}