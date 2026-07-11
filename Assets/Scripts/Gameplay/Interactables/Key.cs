using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour, IInteractable
{
    // Injected dependecies
    private StatesBlackboard blackboard;

    // Private variables
    private Collider baseCollider;

    private bool _initialized = false;

    public void Initialize(StatesBlackboard statesBlackboard)
    {
        this.blackboard = statesBlackboard;
        _initialized = true;
    }

    public void Interact()
    {
        if (!_initialized)
            return;

        Destroy(gameObject);
        blackboard.Set("has_master_key", true);
    }

    public string GetInteractPrompt()
    {
        if (!_initialized)
            return string.Empty;

        return "Take";
    }

    private void OnDrawGizmosSelected()
    {
        if (baseCollider == null)
            baseCollider = GetComponent<Collider>();

        Gizmos.color = ColorProvider.GizmoColors.IInteractableCollider;
        Gizmos.DrawWireCube(baseCollider.bounds.center, baseCollider.bounds.size);
    }
}