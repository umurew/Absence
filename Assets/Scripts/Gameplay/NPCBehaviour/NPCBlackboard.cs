using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCBlackboard", menuName = "Scriptable Objects/NPC Blackboard")]
public class NPCBlackboard : ScriptableObject
{
    [Header("Movement States")]
    [SerializeField] private bool _isSprinting = false;
    [SerializeField] private bool _isCrouching = false;
    [SerializeField] private bool _jumpRequested = false;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _crouchSpeed = 1f;

    public bool IsSprinting => _isSprinting;
    public bool IsCrouching => _isCrouching;
    public bool JumpRequested => _jumpRequested;

    public float MoveSpeed => _moveSpeed;
    public float SprintSpeed => _sprintSpeed;
    public float CrouchSpeed => _crouchSpeed;

    public event Action<NPCBlackboard, Vector3> OnTargetReached;
    public event Action<NPCBlackboard> OnTargetChanged;

    private Vector3? _target = null;
    public Vector3? Target => _target;
    public bool HasTarget => _target.HasValue;

    private void OnEnable() => ResetBlackboard();

    public void SetTarget(Vector3? newTarget)
    {
        if (_target == newTarget)
            return;

        _target = newTarget;
        OnTargetChanged?.Invoke(this);
    }

    public void CompleteTarget()
    {
        if (_target.HasValue)
        {
            Vector3 reachedTarget = _target.Value;
            _target = null;

            OnTargetReached?.Invoke(this, reachedTarget);
            OnTargetChanged?.Invoke(this);
        }
    }

    public void SetSprinting(bool value) => _isSprinting = value;
    public void SetCrouching(bool value) => _isCrouching = value;
    public void RequestJump() => _jumpRequested = true;
    public void ClearJumpRequest() => _jumpRequested = false;

    public void ResetBlackboard()
    {
        _isSprinting = false;
        _isCrouching = false;
        _jumpRequested = false;
        _target = null;
    }
}