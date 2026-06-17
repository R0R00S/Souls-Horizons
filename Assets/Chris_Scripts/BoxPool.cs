using System.Collections.Generic;
using UnityEngine;

public class BoxPool : MonoBehaviour
{
    public static BoxPool Instance;

    public GameObject boxPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        // Pre-create all boxes at startup, then hide them
        for (int i = 0; i < poolSize; i++)
        {
            GameObject box = Instantiate(boxPrefab);
            box.SetActive(false);
            pool.Enqueue(box);
        }
    }

    public GameObject GetBox(Vector3 position)
    {
        if (pool.Count == 0) return null; // pool exhausted

        GameObject box = pool.Dequeue();
        box.transform.position = position;
        box.SetActive(true);
        return box;
    }

    public void ReturnBox(GameObject box)
    {
        box.SetActive(false);
        pool.Enqueue(box);
    }
}