using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class NPCMovement : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1f;
    [Description("Distance to accept target reached.")]
    [SerializeField] private float stoppingDistance = 0.05f;

    [Header("Data Source")]
    public NPCBlackboard blackboard;

    private Animator animator;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isJumping = false;

    private readonly int moveInputXHash = Animator.StringToHash("MoveInputX");
    private readonly int moveInputYHash = Animator.StringToHash("MoveInputY");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int crouchingHash = Animator.StringToHash("Crouching");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (blackboard == null)
            blackboard = new();
    }

    private void Update()
    {
        HandleColliderDimensions();
        Vector3 moveDirection = CalculateMovementDirection();

        HandleGravityAndJump();

        Vector3 finalMovement = moveDirection;
        finalMovement.y = velocity.y;
        controller.Move(finalMovement * Time.deltaTime);

        HandleRotation(moveDirection);
        UpdateAnimationParameters();
    }

    private Vector3 CalculateMovementDirection()
    {
        if (!blackboard.HasTarget)
            return Vector3.zero;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = blackboard.Target.Value;

        Vector3 targetHorizontal = new(targetPosition.x, currentPosition.y, targetPosition.z);
        Vector3 distanceVector = targetHorizontal - currentPosition;
        float distance = distanceVector.magnitude;

        if (distance <= stoppingDistance && !isJumping)
        {
            blackboard.CompleteTarget();
            return Vector3.zero;
        }

        float currentSpeed = GetCurrentSpeed();
        return distanceVector.normalized * currentSpeed;
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero)
            return;

        Vector3 horizontalDirection = new(moveDirection.x, 0f, moveDirection.z);
        if (horizontalDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleColliderDimensions()
    {
        if (blackboard.IsCrouching)
        {
            controller.center = new Vector3(0f, 0.75f, 0f);
            controller.height = 1.3f;
        }
        else
        {
            controller.center = new Vector3(0f, 1.1f, 0f);
            controller.height = 2f;
        }
    }

    private void HandleGravityAndJump()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
                isJumping = false;
            }

            if (blackboard.JumpRequested)
            {
                isJumping = true;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (animator != null)
                    animator.SetTrigger(jumpHash);
            }
        }
        else
            velocity.y += gravity * Time.deltaTime;

        if (blackboard.JumpRequested)
        {
            blackboard.SetCrouching(false);
            blackboard.ClearJumpRequest();
        }
    }

    private float GetCurrentSpeed()
    {
        if (blackboard.IsSprinting)
            return blackboard.SprintSpeed;

        if (blackboard.IsCrouching)
            return blackboard.CrouchSpeed;

        return blackboard.MoveSpeed;
    }

    private void UpdateAnimationParameters()
    {
        if (animator == null)
            return;

        Vector3 localVelocity = transform.InverseTransformDirection(controller.velocity);
        float currentSpeed = GetCurrentSpeed();

        float localX = currentSpeed > 0.01f ? localVelocity.x / currentSpeed : 0f;
        float localZ = currentSpeed > 0.01f ? localVelocity.z / currentSpeed : 0f;

        float modifier = blackboard.IsSprinting ? 2f : 1f;
        animator.SetFloat(moveInputXHash, localX * modifier, 0.1f, Time.deltaTime);
        animator.SetFloat(moveInputYHash, localZ * modifier, 0.1f, Time.deltaTime);

        animator.SetBool(isGroundedHash, controller.isGrounded);
        animator.SetBool(crouchingHash, blackboard.IsCrouching);

        int crouchLayerIndex = animator.GetLayerIndex("Crouch Layer");
        if (crouchLayerIndex != -1)
        {
            float targetWeight = blackboard.IsCrouching ? 1f : 0f;
            float currentWeight = animator.GetLayerWeight(crouchLayerIndex);
            float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * 5f);
            animator.SetLayerWeight(crouchLayerIndex, newWeight);
        }
    }
}