using UnityEngine;
using System.Collections.Generic;

public class FlameInputHandler : MonoBehaviour
{
    private Camera cam;

    // Tracks which touch index is currently extinguishing which flame
    private Dictionary<int, FlameController> activeExtinguishes = new Dictionary<int, FlameController>();

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        if (Input.touchCount > 0)
        {
            HandleTouches();
        }
        else
        {
            // Mouse fallback for testing in the Editor — simulates a single "second finger"
            HandleMouseFallback();
        }
    }

    void HandleTouches()
    {
        // Track which touch indices are still active this frame
        HashSet<int> stillActive = new HashSet<int>();

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            stillActive.Add(i);

            if (touch.phase == TouchPhase.Began)
            {
                TryStartExtinguish(i, touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                StopExtinguish(i);
                stillActive.Remove(i);
            }
        }

        // Clean up any tracked touches that disappeared without an Ended event
        List<int> toRemove = new List<int>();
        foreach (var kvp in activeExtinguishes)
        {
            if (!stillActive.Contains(kvp.Key))
                toRemove.Add(kvp.Key);
        }
        foreach (int index in toRemove)
            StopExtinguish(index);
    }

    void TryStartExtinguish(int touchIndex, Vector3 screenPos)
    {
        // IMPORTANT: touch index 0 is reserved for box dragging in InputManager.
        // Only allow flame extinguishing from touch index 1 and above.
        if (touchIndex == 0) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            FlameController flame = hit.collider.GetComponent<FlameController>();
            if (flame != null)
            {
                flame.StartExtinguishing();
                activeExtinguishes[touchIndex] = flame;
            }
        }
    }

    void StopExtinguish(int touchIndex)
    {
        if (activeExtinguishes.TryGetValue(touchIndex, out FlameController flame))
        {
            flame.StopExtinguishing();
            activeExtinguishes.Remove(touchIndex);
        }
    }

    // --- Editor-only fallback so you can test in Play mode with a mouse ---
    private bool mouseExtinguishing = false;
    private FlameController mouseFlame;

    void HandleMouseFallback()
    {
        if (Input.GetMouseButtonDown(1)) // right-click simulates "second finger"
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                FlameController flame = hit.collider.GetComponent<FlameController>();
                if (flame != null)
                {
                    flame.StartExtinguishing();
                    mouseFlame = flame;
                    mouseExtinguishing = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(1) && mouseExtinguishing)
        {
            mouseFlame.StopExtinguishing();
            mouseExtinguishing = false;
            mouseFlame = null;
        }
    }
}