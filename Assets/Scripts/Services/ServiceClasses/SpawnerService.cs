using System.Collections.Generic;
using UnityEngine;

public class SpawnerService : MonoBehaviour, ISpawnerService
{
    // Injected dependecies
    private ObjectRegistry registry;
    private StatesBlackboard statesBlackboard;

    public IReadOnlyList<string> AvailableObjects => _cachedKeys;
    private List<string> _cachedKeys;

    public void Initialize(ObjectRegistry registry, StatesBlackboard statesBlackboard)
    {
        this.registry = registry;
        this.statesBlackboard = statesBlackboard;

        _cachedKeys = this.registry.GetAvailableKeys();
    }

    public bool TrySpawnObject(string objectId, Vector3 position, Transform parent = null)
    {
        if (!registry.GetPrefab(objectId, out GameObject prefab))
        {
            Debug.LogWarning($"Object with \"{objectId}\" Id not found in Object Registry!");
            return false;
        }

        GameObject cloneObject = Instantiate(prefab, position, Quaternion.identity);

        if (parent != null)
            cloneObject.transform.SetParent(parent);

        if (cloneObject.TryGetComponent<IInteractable>(out IInteractable interactable))
            interactable.Initialize(statesBlackboard);

        return true;
    }
}