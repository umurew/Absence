using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Space(10)]
    public bool sprintAllowed = false;
    public float sprintSpeed = 5f;

    [Space(5)]
    public bool crouchAllowed = false;
    public float crouchSpeed = 1f;

    [Space(5)]
    public bool jumpAllowed = false;
    public float jumpHeight = 1f;

    [Space(5)]
    public float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    [Space(5)]
    public bool noclip = false;
    public float noclipSpeed = 10f;

    private CharacterController characterController;
    private Animator animator;

    private float verticalVelocity = 0f;
    private bool isCrouching = false;

    private readonly int moveInputXHash = Animator.StringToHash("MoveInputX");
    private readonly int moveInputYHash = Animator.StringToHash("MoveInputY");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int crouchingHash = Animator.StringToHash("Crouching");
    private readonly int noclipHash = Animator.StringToHash("Noclip");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        InputManager.Instance.SetCursorState(true);
    }

    private void Update()
    {
        // Handle player rotation with camera
        Vector3 cameraEulerAngles = cameraTransform.eulerAngles;
        transform.rotation = noclip ? Quaternion.Euler(cameraEulerAngles.x, cameraEulerAngles.y, cameraEulerAngles.z) : Quaternion.Euler(0f, cameraEulerAngles.y, 0f);

        // Handle horizontal movement
        Vector2 moveInput = InputManager.Instance.PlayerActions.Move.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        if (!noclip)
        {
            forward.y = 0f;
            right.y = 0f;
        }

        forward.Normalize();
        right.Normalize();

        Vector3 horizontalVelocity = forward * moveInput.y + right * moveInput.x;

        // Handle noclip
        if (noclip)
        {
            Vector3 noclipVelocity = noclipSpeed * Time.deltaTime * horizontalVelocity;

            if (InputManager.Instance.PlayerActions.Jump.IsInProgress())
            {
                Vector3 up = cameraTransform.up;
                up.Normalize();

                noclipVelocity += noclipSpeed * Time.deltaTime * up;
            }
            else if (InputManager.Instance.PlayerActions.Crouch.IsInProgress())
            {
                Vector3 up = cameraTransform.up;
                up.Normalize();

                noclipVelocity -= noclipSpeed * Time.deltaTime * up;
            }

            transform.position += noclipVelocity;

            return;
        }

        // Handle ground check and gravity
        if (characterController.isGrounded)
        {
            /// Reset vertical velocity to prevent infinite falling
            /// Used -2f to keep the controller firmly snapped to slopes and stairs
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            // Handle crouch input if enabled
            if (crouchAllowed && InputManager.Instance.PlayerActions.Crouch.IsInProgress())
            {
                isCrouching = true;

                characterController.center = new Vector3(0f, 0.75f, 0f);
                characterController.height = 1.3f;
            }
            else
            {
                isCrouching = false;

                characterController.center = new Vector3(0f, 1.1f, 0f);
                characterController.height = 2f;
            }

            // Handle jump input if enabled
            if (jumpAllowed && InputManager.Instance.PlayerActions.Jump.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (animator != null)
                    animator.SetTrigger(jumpHash);
            }
        }
        else
            verticalVelocity += gravity * Time.deltaTime;

        // Use sprint speed if allowed and sprint key is held, use move speed otherwise
        bool isSprinting = sprintAllowed && InputManager.Instance.PlayerActions.Sprint.IsInProgress();
        float horizontalSpeed = true switch
        {
            _ when noclip => noclipSpeed,
            _ when isSprinting => sprintSpeed,
            _ when isCrouching => crouchSpeed,
            _ => moveSpeed
        };

        // Apply final velocity
        Vector3 finalVelocity = horizontalSpeed * horizontalVelocity + Vector3.up * verticalVelocity;
        characterController.Move(finalVelocity * Time.deltaTime);

        UpdateParameters(moveInput, isSprinting, isCrouching);
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

        animator.SetBool(isGroundedHash, characterController.isGrounded);
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

    public void ToggleNoclip()
    {
        noclip = !noclip;

        characterController.enabled = !noclip;
        animator.SetBool(noclipHash, noclip);

        if (noclip)
            UpdateParameters(new(0, 0), false, false);
    }
}