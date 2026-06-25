using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera cam;
    private GameObject heldBox;
    private Vector3 dragOffset;
    private Plane dragPlane;

    // Drag plane height � set this to match your floor's Y position
    // No separate liftHeight needed anymore, animation handles visual displacement
    private float dragPlaneHeight = 1.5f;

    void Start()
    {
        cam = Camera.main;
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

        // 0.4f is generous for a finger tap � adjust based on your box size
        if (Physics.SphereCast(ray, 0.4f, out RaycastHit hit, 50f))
        {
            if (hit.collider.CompareTag("Box"))
            {
                heldBox = hit.collider.gameObject;

                dragPlane = new Plane(Vector3.up, new Vector3(0, dragPlaneHeight, 0));

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
            targetPos.y = dragPlaneHeight;

            // Smooth follow
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
    }

    // --- input helpers unchanged ---
    Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0) return Input.GetTouch(0).position;
        return Input.mousePosition;
    }

    bool GetPressedThisFrame()
    {
        if (Input.touchCount > 0) return Input.GetTouch(0).phase == TouchPhase.Began;
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
        if (Input.touchCount > 0) return Input.GetTouch(0).phase == TouchPhase.Ended;
        return Input.GetMouseButtonUp(0);
    }
}