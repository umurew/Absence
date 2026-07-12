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

    private bool _initialized = false;

    public void Initialize()
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(ObjectRegistry)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _registryCache = new(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Id) && item.Prefab != null)
                _registryCache[item.Id] = item.Prefab;
        }

        _initialized = true;
    }

    public bool GetPrefab(string objectId, out GameObject gameObject)
    {
        InitializedCheck();

        return _registryCache.TryGetValue(objectId, out gameObject);
    }

    public List<string> GetAvailableKeys()
    {
        InitializedCheck();

        return _registryCache.Keys.ToList();
    }

    private void OnEnable() => _initialized = false;

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(ObjectRegistry)} must be initialized before use.");
    }
}