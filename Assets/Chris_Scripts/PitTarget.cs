using UnityEngine;


public class PitTarget : MonoBehaviour
{
    public int pitType;
    public Color pitColor; // set this to match the corresponding box color

    void Start()
    {
        // Optionally tint the pit itself so it visually matches its boxes
        Renderer r = GetComponent<Renderer>();
        if (r != null) r.material.color = pitColor;
    }
}