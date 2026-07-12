using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatesBlackboard", menuName = "Scriptable Objects/States Blackboard")]
public class StatesBlackboard : ScriptableObject
{
    private bool _initialized = false;
    private readonly Dictionary<string, object> StateDictionary = new();

    public void Initialize()
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(StatesBlackboard)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _initialized = true;
    }

    public void Set(string Key, object Value)
    {
        InitializedCheck();

        StateDictionary[Key] = Value;
        Debug.Log($"State set with the key \"{Key}\" to {Value}");
    }

    public T Get<T>(string Key)
    {
        InitializedCheck();

        if (StateDictionary.TryGetValue(Key, out object Value))
            return (T)Value;

        Debug.Log($"State with the key \"{Key}\" is being read");
        return default;
    }

    public void ResetStates() => StateDictionary.Clear();

    private void OnEnable() => _initialized = false;

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(StatesBlackboard)} must be initialized before use.");
    }
}