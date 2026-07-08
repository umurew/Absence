using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnRegistry", menuName = "Scriptable Objects/SpawnRegistry")]
public class SpawnRegistry : ScriptableObject
{
    [Serializable]
    public struct SpawnableItem
    {
        public string commandId;
        public GameObject prefab;
    }

    [SerializeField] private List<SpawnableItem> items;
    private Dictionary<string, GameObject> _registryCache;

    public void Initialize()
    {
        _registryCache = new(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.commandId) && item.prefab != null)
                _registryCache[item.commandId] = item.prefab;
        }
    }
    public bool GetPrefab(string objectId, out GameObject gameObject)
    {
        if (_registryCache == null)
            Initialize();

        return _registryCache.TryGetValue(objectId, out gameObject);
    }

    public List<string> GetAvailableKeys()
    {
        if (_registryCache == null)
            Initialize();

        return _registryCache.Keys.ToList();
    }
}