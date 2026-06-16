using UnityEngine;
using System.Collections;

public class SoulSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject soulPrefab;
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 4f;

    [Header("Sin Weights")]
    [Range(0, 100)] public int innocentChance = 20;
    [Range(0, 100)] public int greedChance    = 40;
    [Range(0, 100)] public int wrathChance    = 40;

    [Header("Soul Materials")]
    public Material innocentMaterial;
    public Material greedMaterial;
    public Material wrathMaterial;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);
            SpawnSoul();
        }
    }

    void SpawnSoul()
    {
        SinType sin = PickSinType();

        Vector3 spawnPos = transform.position + new Vector3(0, 0, Random.Range(-0.5f, 0.5f));
        GameObject soulObj = Instantiate(soulPrefab, spawnPos, Quaternion.identity);
        Soul soul = soulObj.GetComponent<Soul>();
        soul.sinType = sin;

        Renderer rend = soulObj.GetComponent<Renderer>();
        rend.material = sin switch
        {
            SinType.Greed => greedMaterial,
            SinType.Wrath => wrathMaterial,
            _             => innocentMaterial,
        };
    }

    SinType PickSinType()
    {
        int total = innocentChance + greedChance + wrathChance;
        int roll  = Random.Range(0, total);

        if (roll < innocentChance)               return SinType.None;
        if (roll < innocentChance + greedChance) return SinType.Greed;
        return SinType.Wrath;
    }
}