using UnityEngine;

public class GateTarget : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        BoxDraggable box = other.GetComponent<BoxDraggable>();
        if (box == null) return;
        if (box.isBeingHeld) return;

        // Time soul reached the gate � time punishment
        if (box.isTimeSoul)
        {
            GameManager.Instance.WrongPit();
            GameManager.Instance.ModifyTime(
                GameManager.Instance.currentLevel.timeSoulPenaltySeconds
            );
            BoxSpawner.Instance.BoxSorted();
            if (TimeSoulSpawner.Instance != null)
                TimeSoulSpawner.Instance.TimeSoulResolved(other.gameObject);
            return;
        }

        if (box.boxType == BoxType.None)
        {
            GameManager.Instance.CorrectPit(other.gameObject);
        }
        else
        {
            GameManager.Instance.WrongPit();
            BoxPool.Instance.ReturnBox(other.gameObject);
            BoxSpawner.Instance.BoxSorted();
        }
    }
}