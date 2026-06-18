using UnityEngine;

public class GateTarget : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Only care about objects with BoxDraggable on them
        BoxDraggable box = other.GetComponent<BoxDraggable>();
        if (box == null) return;

        // Ignore boxes the player is currently holding
        if (box.isBeingHeld) return;

        if (box.boxType == BoxType.None)
        {
            Debug.Log("Safe box passed through gate correctly.");
            GameManager.Instance.CorrectPit(other.gameObject);
        }
        else
        {
            Debug.Log($"Wrong box ({box.boxType}) reached the gate — lose a life.");
            GameManager.Instance.WrongPit();
            BoxPool.Instance.ReturnBox(other.gameObject);
            BoxSpawner.Instance.BoxSorted();
        }
    }
}