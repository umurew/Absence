using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isLocked = false;
    [SerializeField] private float openAngle = -100f;
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private float smoothSpeed = 5f;

    private StatesBlackboard _statesBlackboard;

    private Collider _collider;
    private bool _isDoorOpened = false;
    private bool _debounce = false;
    private bool _initialized = false;
    private Quaternion _targetRotation;
    private readonly static WaitForSeconds _waitForSeconds0_3 = new(0.3f);

    public void Initialize(StatesBlackboard statesBlackboard)
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(Door)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _statesBlackboard = statesBlackboard;
        _targetRotation = Quaternion.Euler(0f, closedAngle, 0f);
        _initialized = true;
    }

    public void Interact()
    {
        InitializedCheck();

        if (_debounce)
            return;

        if (isLocked)
        {
            if (!_statesBlackboard.Get<bool>("has_master_key"))
                return;

            isLocked = false;
            return;
        }

        StartCoroutine(InteractionDebounceRoutine());

        _isDoorOpened = !_isDoorOpened;
        float targetAngle = _isDoorOpened ? openAngle : closedAngle;
        _targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    public string GetInteractPrompt()
    {
        InitializedCheck();

        if (_debounce)
            return "...";

        if (isLocked)
            return _statesBlackboard.Get<bool>("has_master_key") ? "Unlock" : "It's Locked";

        return _isDoorOpened ? "Close" : "Open";
    }

    private void Update()
    {
        if (!_initialized)
            return;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, _targetRotation, Time.deltaTime * smoothSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        Gizmos.color = ColorProvider.GizmoColors.IInteractableCollider;
        Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);

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

            UnityEditor.Handles.Label(labelPosition, isLocked ? "LOCKED" : "UNLOCKED", style);
        }
#endif
    }

    private IEnumerator InteractionDebounceRoutine()
    {
        _debounce = true;

        yield return _waitForSeconds0_3;

        _debounce = false;
    }

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(Door)} must be initialized before use.");
    }
}