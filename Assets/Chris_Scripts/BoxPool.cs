using UnityEngine;
using System.Collections.Generic;

public class BoxPool : MonoBehaviour
{
    public static BoxPool Instance;

    public GameObject boxPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject box = Instantiate(boxPrefab);
            box.SetActive(false);
            pool.Enqueue(box);
        }
    }

    public GameObject GetBox(Vector3 position)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool exhausted — increase pool size");
            return null;
        }

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