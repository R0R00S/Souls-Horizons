using UnityEngine;
using System.Collections.Generic;

public class BoxPool : MonoBehaviour
{
    public static BoxPool Instance;

    [Header("One prefab per box type — assign in Inspector")]
    public GameObject[] boxPrefabs; // slot 0 = None, 1 = TypeA, 2 = TypeB etc.
    public int poolSizePerType = 5;

    // Separate queue per prefab type
    private Dictionary<BoxType, Queue<GameObject>> pools
        = new Dictionary<BoxType, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;

        foreach (GameObject prefab in boxPrefabs)
        {
            BoxType type = prefab.GetComponent<BoxDraggable>().boxType;

            if (!pools.ContainsKey(type))
                pools[type] = new Queue<GameObject>();

            for (int i = 0; i < poolSizePerType; i++)
            {
                GameObject box = Instantiate(prefab);
                box.SetActive(false);
                pools[type].Enqueue(box);
            }
        }
    }

    public GameObject GetBox(BoxType type, Vector3 position)
    {
        if (!pools.ContainsKey(type) || pools[type].Count == 0)
        {
            
            return null;
        }

        GameObject box = pools[type].Dequeue();
        box.transform.position = position;
        box.SetActive(true);
        return box;
    }

    public void ReturnBox(GameObject box)
    {
        BoxType type = box.GetComponent<BoxDraggable>().boxType;
        box.SetActive(false);

        if (pools.ContainsKey(type))
            pools[type].Enqueue(box);
    }
}