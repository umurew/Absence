using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerService : MonoBehaviour, ISpawnerService
{
    private ObjectRegistry _objectRegistry;
    private StatesBlackboard _statesBlackboard;
    private List<string> _cachedKeys;
    private bool _initialized = false;

    public IReadOnlyList<string> AvailableObjects => _cachedKeys;

    public void Initialize(ObjectRegistry registry, StatesBlackboard statesBlackboard)
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(SpawnerService)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _objectRegistry = registry;
        _statesBlackboard = statesBlackboard;

        _cachedKeys = _objectRegistry.GetAvailableKeys();

        _initialized = true;
    }

    public bool TrySpawnObject(string objectId, Vector3 position, Transform parent = null)
    {
        InitializedCheck();

        if (string.IsNullOrEmpty(objectId))
        {
            Debug.LogWarning($"{nameof(SpawnerService)}: Cannot spawn an object without an Id.");
            return false;
        }

        if (!_objectRegistry.GetPrefab(objectId, out GameObject prefab))
        {
            Debug.LogWarning($"{nameof(SpawnerService)}: Object with \"{objectId}\" Id not found in Object Registry!");
            return false;
        }

        GameObject cloneObject = Instantiate(prefab, position, Quaternion.identity);

        if (parent != null)
            cloneObject.transform.SetParent(parent);

        if (cloneObject.TryGetComponent<IInteractable>(out IInteractable interactable))
            interactable.Initialize(_statesBlackboard);

        return true;
    }

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(SpawnerService)} must be initialized before use.");
    }
}