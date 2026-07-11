using System.Collections.Generic;
using UnityEngine;

public interface ISpawnerService
{
    IReadOnlyList<string> AvailableObjects { get; }
    bool TrySpawnObject(string objectId, Vector3 position, Transform parent = null);
}