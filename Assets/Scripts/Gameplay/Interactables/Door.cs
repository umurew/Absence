using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    // Serialized variables
    [SerializeField] private bool locked = false;
    [SerializeField] private float openAngle = -100f;
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private float smoothSpeed = 5f;

    // Injected dependecies
    private StatesBlackboard blackboard;

    // Private variables
    private Collider baseCollider;
    private bool doorState = false;
    private bool debounce = false;
    private Quaternion targetRotation;
    private readonly static WaitForSeconds _waitForSeconds0_3 = new(0.3f);

    private bool _initialized = false;

    public void Initialize(StatesBlackboard statesBlackboard)
    {
        this.blackboard = statesBlackboard;
        _initialized = true;
    }

    private void Start() => targetRotation = Quaternion.Euler(0f, closedAngle, 0f);

    private void Update() => transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);

    public void Interact()
    {
        if (!_initialized || debounce)
            return;

        if (locked)
        {
            if (!blackboard.Get<bool>("has_master_key"))
                return;

            locked = false;
        }

        StartCoroutine(InteractionDebounceRoutine());

        doorState = !doorState;
        float targetAngle = doorState ? openAngle : closedAngle;
        targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    public string GetInteractPrompt()
    {
        if (!_initialized || debounce)
            return string.Empty;

        if (locked)
            return blackboard.Get<bool>("has_master_key") ? "Unlock" : "It's Locked";

        return doorState ? "Close" : "Open";
    }

    private IEnumerator InteractionDebounceRoutine()
    {
        debounce = true;
        yield return _waitForSeconds0_3;
        debounce = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (baseCollider == null)
            baseCollider = GetComponent<Collider>();

        Gizmos.color = ColorProvider.GizmoColors.IInteractableCollider;
        Gizmos.DrawWireCube(baseCollider.bounds.center, baseCollider.bounds.size);

        Vector3 position = transform.position;

        Gizmos.color = ColorProvider.GizmoColors.DoorAxis;
        Vector3 axisStart = position - transform.up * 0.25f;
        Vector3 axisEnd = position + transform.up * 2.5f;
        Gizmos.DrawLine(axisStart, axisEnd);

        Gizmos.color = ColorProvider.GizmoColors.DoorArrow;
        Vector3 arrowBase = position + transform.right * 1.125f + transform.up * 1.125f;
        Vector3 arrowTip = arrowBase + transform.forward * 0.2f;

        Gizmos.DrawLine(arrowBase - transform.forward * 0.2f, arrowBase + transform.forward * 0.2f);

        Vector3 leftFlank = arrowTip - transform.right * 0.1f - transform.forward * 0.1f;
        Gizmos.DrawLine(leftFlank, arrowTip);

        Vector3 rightFlank = arrowTip + transform.right * 0.1f - transform.forward * 0.1f;
        Gizmos.DrawLine(rightFlank, arrowTip);

#if UNITY_EDITOR
        if (UnityEditor.SceneView.currentDrawingSceneView == null || UnityEditor.SceneView.currentDrawingSceneView.camera == null)
            return;

        Transform sceneCamera = UnityEditor.SceneView.currentDrawingSceneView.camera.transform;
        float distance = Vector3.Distance(sceneCamera.position, position);

        if (distance <= 10f)
        {
            Vector3 labelPosition = position + transform.right * 0.625f + transform.up * 1.125f;

            GUIStyle style = new()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            style.normal.textColor = ColorProvider.GizmoColors.HandleLabel;

            UnityEditor.Handles.Label(labelPosition, locked ? "LOCKED" : "UNLOCKED", style);
        }
#endif
    }
}