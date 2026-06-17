using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera cam;
    private GameObject heldBox;
    private Vector3 dragOffset;
    private Plane dragPlane;

    private float dragHeight = 1.5f;  // height box floats while held
    private float liftHeight = 0.6f;  // extra lift so finger doesn't cover box

    void Start()
    {
        cam = Camera.main; // cached once — not called every frame
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        bool pressedThisFrame = GetPressedThisFrame();
        bool pressing = GetPressing();
        bool releasedThisFrame = GetReleasedThisFrame();

        if (pressedThisFrame) TryGrabBox();
        if (pressing && heldBox != null) DragBox();
        if (releasedThisFrame && heldBox != null) ReleaseBox();
    }

    void TryGrabBox()
    {
        Ray ray = cam.ScreenPointToRay(GetInputPosition());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Box"))
            {
                heldBox = hit.collider.gameObject;

                // Drag plane sits at the float height
                dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));

                // Offset so box doesn't snap to finger position
                if (dragPlane.Raycast(ray, out float dist))
                {
                    dragOffset = heldBox.transform.position - ray.GetPoint(dist);
                    dragOffset.y = 0;
                }

                heldBox.GetComponent<BoxDraggable>().OnPickUp();
                Debug.Log("Grabbed: " + heldBox.name);
            }
        }
    }

    void DragBox()
    {
        Ray ray = cam.ScreenPointToRay(GetInputPosition());

        if (dragPlane.Raycast(ray, out float dist))
        {
            Vector3 targetPos = ray.GetPoint(dist) + dragOffset;

            // Lift box above finger so it stays visible on mobile
            targetPos.y = dragHeight + liftHeight;

            // Smooth movement instead of instant teleport
            heldBox.transform.position = Vector3.Lerp(
                heldBox.transform.position,
                targetPos,
                Time.deltaTime * 20f
            );
        }
    }

    void ReleaseBox()
    {
        heldBox.GetComponent<BoxDraggable>().OnRelease();
        heldBox = null;
        Debug.Log("Released box");
    }

    // --- Input helpers: resolve touch vs mouse once per frame ---

    Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        return Input.mousePosition;
    }

    bool GetPressedThisFrame()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;
        return Input.GetMouseButtonDown(0);
    }

    bool GetPressing()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Moved
                || Input.GetTouch(0).phase == TouchPhase.Stationary;
        return Input.GetMouseButton(0);
    }

    bool GetReleasedThisFrame()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Ended;
        return Input.GetMouseButtonUp(0);
    }
}