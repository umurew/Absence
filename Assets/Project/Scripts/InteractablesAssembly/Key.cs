using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private StatesBlackboard blackboard;
    private Collider baseCollider;

    public void Interact()
    {
        Destroy(gameObject);
        blackboard.Set("has_master_key", true);
    }

    public string GetInteractPrompt() => "Take";

    private void OnDrawGizmosSelected()
    {
        if (baseCollider == null)
            baseCollider = GetComponent<Collider>();

        Gizmos.color = ColorProvider.GizmoColors.IInteractableCollider;
        Gizmos.DrawWireCube(baseCollider.bounds.center, baseCollider.bounds.size);
    }
}