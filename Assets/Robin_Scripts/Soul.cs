using UnityEngine;

public enum SinType { None, Greed, Wrath }

public class Soul : MonoBehaviour
{
    public SinType sinType = SinType.None;
    public float moveSpeed = 2f;

    [HideInInspector] public bool isHeld = false;

    Transform elysiumTarget;

    void Start()
    {
        // Find the Elysium gate by tag
        elysiumTarget = GameObject.FindWithTag("Elysium").transform;
    }

    void Update()
    {
        if (isHeld) return;

        // Move toward Elysium along the path (X axis only, keep Z and Y fixed)
        Vector3 target = new Vector3(
            elysiumTarget.position.x,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void PickUp()  => isHeld = true;
    public void Release()
    {
        isHeld = false;
        // Snap back to path Z and correct height
        transform.position = new Vector3(
            transform.position.x,
            0.5f,
            5f        // match your path's Z position
        );
    }
}