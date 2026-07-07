using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private StatesBlackboard blackboard;
    private Collider baseCollider;

    private void Start() => baseCollider = GetComponent<Collider>();

    public void Interact()
    {
        Destroy(gameObject);
        blackboard.Set("has_master_key", true);
    }

    public string GetInteractPrompt() => "Take";

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = ColorProvider.Instance.GizmoColors.IInteractableCollider;
        Gizmos.DrawCube(baseCollider.bounds.center, baseCollider.bounds.size);
    }
}