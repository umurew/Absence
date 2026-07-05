using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private bool jumpAllowed = false;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalVelocity = 0f;

    private void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        InputManager.Instance.SetCursorState(true);
    }

    private void Update()
    {
        // Handle horizontal movement
        Vector2 moveInput = InputManager.Instance.playerActions.Move.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 horizontalVelocity = forward * moveInput.y + right * moveInput.x;

        // Handle ground check and gravity
        if (characterController.isGrounded)
        {
            /// Reset vertical velocity to prevent infinite falling
            /// Used -2f to keep the controller firmly snapped to slopes and stairs
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            // Handle jump input if enabled
            if (jumpAllowed && InputManager.Instance.playerActions.Jump.WasPressedThisFrame())
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
            verticalVelocity += gravity * Time.deltaTime;

        // Apply final velocity
        Vector3 finalVelocity = moveSpeed * horizontalVelocity + Vector3.up * verticalVelocity;
        characterController.Move(finalVelocity * Time.deltaTime);
    }
}