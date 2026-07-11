using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectRegistry", menuName = "Scriptable Objects/Object Registry")]
public class ObjectRegistry : ScriptableObject
{
    [Serializable] public struct ObjectData
    {
        public string Id;
        public GameObject Prefab;
    }

    [SerializeField] private List<ObjectData> items;
    private Dictionary<string, GameObject> _registryCache;

    public void Initialize()
    {
        _registryCache = new(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Id) && item.Prefab != null)
                _registryCache[item.Id] = item.Prefab;
        }
    }

    public bool GetPrefab(string objectId, out GameObject gameObject) => _registryCache.TryGetValue(objectId, out gameObject);

    public List<string> GetAvailableKeys() => _registryCache.Keys.ToList();
}