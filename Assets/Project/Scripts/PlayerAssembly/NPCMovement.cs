using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class NPCMovement : MonoBehaviour
{
    [Header("Behaviour")]
    public bool wanderAround = false;
    [SerializeField] private float wanderInterval = 5f;
    [SerializeField] private float wanderRadius = 5f;

    [Space(10)]
    public bool isSprinting = false;
    public float sprintSpeed = 5f;

    [Space(5)]
    public bool isCrouching = false;
    public float crouchSpeed = 1f;

    [Space(5)]
    private bool isJumping = false;
    public float jumpHeight = 1f;

    [Space(5)]
    public float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    private NavMeshAgent agent;
    private Animator animator;
    private CharacterController controller;
    private float timer;
    private Vector3 jumpVelocity;

    private readonly int moveInputXHash = Animator.StringToHash("MoveInputX");
    private readonly int moveInputYHash = Animator.StringToHash("MoveInputY");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int crouchingHash = Animator.StringToHash("Crouching");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        agent.updatePosition = false;
        timer = wanderInterval;
    }

    private void Update()
    {
        // Handle timer
        if (wanderAround)
        {
            timer += Time.deltaTime;
            if (timer >= wanderInterval)
            {
                Vector3 newTargetPosition = GetRandomNavPosition(transform.position, wanderRadius, NavMesh.AllAreas);
                agent.SetDestination(newTargetPosition);
                timer = 0;
            }
        }

        // Handle isJumping and gravity
        if (!isJumping)
        {
            agent.nextPosition = transform.position;

            Vector3 moveVelocity = agent.velocity;
            moveVelocity.y = -2f;

            controller.Move(moveVelocity * Time.deltaTime);
        }
        else
        {
            jumpVelocity.y += gravity * Time.deltaTime;
            controller.Move(jumpVelocity * Time.deltaTime);

            if (controller.isGrounded && jumpVelocity.y < 0)
            {
                isJumping = false;
                agent.updateRotation = true;
                agent.Warp(transform.position);
            }
        }

        // Handle crouching
        if (isCrouching)
        {
            controller.center = new Vector3(0f, 0.75f, 0f);
            controller.height = 1.3f;
        }
        else
        {
            controller.center = new Vector3(0f, 1.1f, 0f);
            controller.height = 2f;
        }

        float horizontalSpeed = true switch
        {
            _ when isSprinting => sprintSpeed,
            _ when isCrouching => crouchSpeed,
            _ => moveSpeed
        };

        agent.speed = horizontalSpeed;

        // Handle walking animation
        Vector3 worldVelocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

        float localX = localVelocity.x / agent.speed;
        float localZ = localVelocity.z / agent.speed;

        UpdateParameters(new Vector2(localX, localZ), isSprinting, isCrouching);
    }

    private void UpdateParameters(Vector2 moveInput, bool isSprinting, bool isCrouching)
    {
        if (animator == null)
        {
            Debug.LogError("Animator was null");
            return;
        }

        float modifier = isSprinting ? 2f : 1f;
        animator.SetFloat(moveInputXHash, moveInput.x * modifier, 0.1f, Time.deltaTime);
        animator.SetFloat(moveInputYHash, moveInput.y * modifier, 0.1f, Time.deltaTime);

        animator.SetBool(isGroundedHash, controller.isGrounded);
        animator.SetBool(crouchingHash, isCrouching);

        int crouchLayerIndex = animator.GetLayerIndex("Crouch Layer");

        if (crouchLayerIndex != -1)
        {
            float targetWeight = isCrouching ? 1f : 0f;
            float currentWeight = animator.GetLayerWeight(crouchLayerIndex);

            float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * 5f);
            animator.SetLayerWeight(crouchLayerIndex, newWeight);
        }
    }

    public void Jump()
    {
        if (isJumping || !controller.isGrounded)
            return;

        isJumping = true;
        agent.updateRotation = false;

        float verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        jumpVelocity = agent.velocity;
        jumpVelocity.y = verticalVelocity;

        animator.SetTrigger(jumpHash);
    }

    Vector3 GetRandomNavPosition(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layerMask))
            return navHit.position;
        else
            return origin;
    }
}