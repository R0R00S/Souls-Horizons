using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera cam;
    private GameObject heldBox;
    private Vector3 dragOffset;
    private Plane dragPlane;

    public static InputManager Instance;

    // Drag plane height — set this to match your floor's Y position
    // No separate liftHeight needed anymore, animation handles visual displacement
    private float dragPlaneHeight = 1.4f;

    [Header("Drag Bounds — set to match your map edges")]
    public float boundsMinX = -5f;
    public float boundsMaxX = 5f;
    public float boundsMinZ = -3f;
    public float boundsMaxZ = 3f;

    void Start()
    {
        Instance = this;
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

    [Header("Layer Settings")]
    public LayerMask draggableLayer; // assign in Inspector — select the Draggable layer

    void TryGrabBox()
    {
        Ray ray = cam.ScreenPointToRay(GetInputPosition());

        // Only hits objects on the Draggable layer — ignores pits, floor, walls, everything else
        if (Physics.SphereCast(ray, 0.4f, out RaycastHit hit, 50f, draggableLayer))
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

            // Clamp to map bounds so box can't be dragged through walls
            targetPos.x = Mathf.Clamp(targetPos.x, boundsMinX, boundsMaxX);
            targetPos.z = Mathf.Clamp(targetPos.z, boundsMinZ, boundsMaxZ);

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

    // Add to InputManager.cs
    public void ForceRelease()
    {
        if (heldBox == null) return;

        heldBox.GetComponent<BoxDraggable>().OnRelease();
        heldBox = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            (boundsMinX + boundsMaxX) / 2f,
            dragPlaneHeight,
            (boundsMinZ + boundsMaxZ) / 2f
        );
        Vector3 size = new Vector3(
            boundsMaxX - boundsMinX,
            0.1f,
            boundsMaxZ - boundsMinZ
        );
        Gizmos.DrawWireCube(center, size);
    }
}