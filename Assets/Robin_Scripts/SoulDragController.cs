using UnityEngine;
using UnityEngine.InputSystem;

public class SoulDragController : MonoBehaviour
{
    Camera cam;
    Soul heldSoul;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)  TryPickUp();
        if (Mouse.current.leftButton.isPressed && heldSoul != null) DragSoul();
        if (Mouse.current.leftButton.wasReleasedThisFrame && heldSoul != null) TryDrop();
    }

    void TryPickUp()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Soul soul = hit.collider.GetComponent<Soul>();
            if (soul != null && !soul.isHeld)
            {
                heldSoul = soul;
                heldSoul.PickUp();
            }
        }
    }

    void DragSoul()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, heldSoul.transform.position.y, 0));
        if (dragPlane.Raycast(ray, out float distance))
            heldSoul.transform.position = ray.GetPoint(distance);
    }

    void TryDrop()
    {
        Collider[] hits = Physics.OverlapSphere(heldSoul.transform.position, 0.8f);

        foreach (Collider col in hits)
        {
            Pit pit = col.GetComponentInParent<Pit>();  // <- this line changed

            if (pit != null)
            {
                pit.ReceiveSoul(heldSoul);
                heldSoul = null;
                return;
            }
        }

        heldSoul.Release();
        heldSoul = null;
    }

    void OnDrawGizmos()
    {
        if (heldSoul != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(heldSoul.transform.position, 0.8f);
        }
    }
}