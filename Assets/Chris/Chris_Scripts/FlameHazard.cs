using UnityEngine;

public class FlameHazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        BoxDraggable box = other.GetComponent<BoxDraggable>();

        if (box == null) return;
        if (!box.isBeingHeld) return;

        Debug.Log("Soul touched flame — time penalty applied");

        // Force release and remove the soul
        InputManager.Instance.ForceRelease();
        BoxSpawner.Instance.BoxSorted();
        BoxPool.Instance.ReturnBox(other.gameObject);

        // Add time as punishment instead of losing a life
        GameManager.Instance.ModifyTime(GameManager.Instance.currentLevel.flamePenaltySeconds);
    }
}