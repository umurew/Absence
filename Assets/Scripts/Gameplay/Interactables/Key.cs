using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour, IInteractable
{
    private StatesBlackboard _statesBlackboard;
    private Collider _collider;
    private bool _initialized = false;

    public void Initialize(StatesBlackboard statesBlackboard)
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(Key)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _statesBlackboard = statesBlackboard;
        _initialized = true;
    }

    public void Interact()
    {
        InitializedCheck();

        Destroy(gameObject);
        _statesBlackboard.Set("has_master_key", true);
    }

    public string GetInteractPrompt()
    {
        InitializedCheck();

        return "Take";
    }

    private void OnDrawGizmosSelected()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        Gizmos.color = ColorProvider.GizmoColors.IInteractableCollider;
        Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
    }

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(Door)} must be initialized before use.");
    }
}