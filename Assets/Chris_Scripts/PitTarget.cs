using UnityEngine;

public class PitTarget : MonoBehaviour
{
    public int pitType;
    public Color pitColor;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null) r.material.color = pitColor;
    }
}