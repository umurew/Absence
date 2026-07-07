using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Space(10)]
    [SerializeField] private bool sprintAllowed = false;
    [SerializeField] public float sprintSpeed = 5f;

    /*[Space(10)]
    [SerializeField] private bool crouchAllowed = false;
    [SerializeField] public float crouchSpeed = 1f;*/

    [Space(10)]
    [SerializeField] private bool jumpAllowed = false;
    [SerializeField] public float jumpHeight = 1f;

    [Space(10)]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    [Space(10)]
    [SerializeField] public bool noclip = false;
    [SerializeField][Range(1f, 20f)] public float noclipSpeed = 10f;

    private CharacterController characterController;
    private Animator animator;
    private float verticalVelocity = 0f;
    private readonly int moveInputXHash = Animator.StringToHash("MoveInputX");
    private readonly int moveInputYHash = Animator.StringToHash("MoveInputY");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
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
        float horizontalSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Apply final velocity
        Vector3 finalVelocity = horizontalSpeed * horizontalVelocity + Vector3.up * verticalVelocity;
        characterController.Move(finalVelocity * Time.deltaTime);

        UpdateParameters(moveInput, isSprinting);
    }

    private void UpdateParameters(Vector2 moveInput, bool isSprinting)
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
    }

    public void ToggleNoclip()
    {
        noclip = !noclip;

        characterController.enabled = !noclip;
        animator.SetBool(noclipHash, noclip);

        if (noclip)
            UpdateParameters(new(0, 0), false);
    }
}