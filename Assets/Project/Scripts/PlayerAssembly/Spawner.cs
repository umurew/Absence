using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; set; }

    [SerializeField] private SpawnRegistry registry;

    [Space(10)]
    [SerializeField] private GameObject environmentRoot;
    [SerializeField] private GameObject player;
    [SerializeField] private float spawnDistance = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TrySpawnObject(string objectId)
    {
        if (!registry.GetPrefab(objectId, out GameObject prefab))
            return false;

        Vector3 spawnPosition = player.transform.position + (player.transform.forward * spawnDistance) + (player.transform.up * 1.85f);

        GameObject cloneObject = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
        cloneObject.transform.parent = environmentRoot.transform;

        return true;
    }

    public List<string> GetAvailableObjects() => registry.GetAvailableKeys();
}