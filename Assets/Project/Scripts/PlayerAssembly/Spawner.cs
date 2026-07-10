using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; set; }

    [SerializeField] private SpawnRegistry registry;

    [Space(10)]
    [SerializeField] private GameObject environmentRoot;
    [SerializeField] private GameObject player;

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

    public bool TrySpawnObject(string objectId, Vector3 position)
    {
        if (!registry.GetPrefab(objectId, out GameObject prefab))
            return false;

        GameObject cloneObject = GameObject.Instantiate(prefab, position, Quaternion.identity);
        cloneObject.transform.parent = environmentRoot.transform;

        return true;
    }

    public List<string> GetAvailableObjects() => registry.GetAvailableKeys();
}