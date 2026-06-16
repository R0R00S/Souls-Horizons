using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameObject heldBox;
    private Vector3 dragOffset;
    private float dragHeight = 1.5f; // how high the box floats while dragged

    // This is the flat plane the box slides along while being dragged
    private Plane dragPlane;

    void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        // Works on both PC (mouse) and mobile (touch) via this helper:
        bool pressing = Input.touchCount > 0 || Input.GetMouseButton(0);
        bool pressedThisFrame = Input.touchCount > 0
            ? Input.GetTouch(0).phase == TouchPhase.Began
            : Input.GetMouseButtonDown(0);
        bool releasedThisFrame = Input.touchCount > 0
            ? Input.GetTouch(0).phase == TouchPhase.Ended
            : Input.GetMouseButtonUp(0);

        if (pressedThisFrame) TryGrabBox();
        if (pressing && heldBox != null) DragBox();
        if (releasedThisFrame && heldBox != null) ReleaseBox();
    }

    void TryGrabBox()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetInputPosition());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Box"))
            {
                heldBox = hit.collider.gameObject;

                // The drag plane sits at the floating height
                dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));

                // Calculate offset so box doesn't snap to finger
                if (dragPlane.Raycast(ray, out float dist))
                {
                    dragOffset = heldBox.transform.position - ray.GetPoint(dist);
                    dragOffset.y = 0; // only offset on XZ
                }

                heldBox.GetComponent<BoxDraggable>().OnPickUp();
                Debug.Log("Grabbed: " + heldBox.name);
            }
        }
    }

    void DragBox()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetInputPosition());
        if (dragPlane.Raycast(ray, out float dist))
        {
            Vector3 targetPos = ray.GetPoint(dist) + dragOffset;
            targetPos.y = dragHeight; // lock Y so it floats
            heldBox.transform.position = targetPos;
        }
    }

    void ReleaseBox()
    {
        heldBox.GetComponent<BoxDraggable>().OnRelease();
        heldBox = null;
        Debug.Log("Released box");
    }

    Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        return Input.mousePosition;
    }
}