using UnityEngine;

public class GateTarget : MonoBehaviour
{
    // Boxes of type None are safe to enter — everything else costs a life
    void OnTriggerEnter(Collider other)
    {
        BoxDraggable box = other.GetComponent<BoxDraggable>();
        if (box == null) return;
        if (box.isBeingHeld) return; // player is still dragging it, ignore

        if (box.boxType == BoxType.None)
        {
            // Safe box reached the gate — correct outcome, no penalty
            Debug.Log("Safe box passed through the gate correctly.");
            GameManager.Instance.CorrectPit(other.gameObject);
        }
        else
        {
            // Wrong box slipped through — player was too slow
            Debug.Log($"Wrong box ({box.boxType}) reached the gate! Lose a life.");
            GameManager.Instance.WrongPit();
            BoxPool.Instance.ReturnBox(other.gameObject);
        }

        BoxSpawner.Instance.BoxSorted();
    }
}