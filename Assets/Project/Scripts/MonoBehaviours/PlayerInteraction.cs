using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Header("Raycast Configuration")]
    [Description("Reach distance which determines maximum raycast lenght.")]
    [SerializeField] private float raycastDistance = 3.5f;
    [Description("Sphere radius which determines the raycast sphere's radius.")]
    [SerializeField] private float raycastRadius = 0.2f;

    [Header("UI References")]
    [SerializeField] private UIDocument hudDocument;

    private IInteractable currentInteractable;
    private VisualElement promptContanier;
    private Label promptLabel;
    private Label promptHeaderLabel;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (hudDocument != null)
        {
            VisualElement root = hudDocument.rootVisualElement;
            promptContanier = root.Q<VisualElement>("PromptLabelContainer");
            promptLabel = root.Q<Label>("PromptLabel");
            promptHeaderLabel = root.Q<Label>("PromptHeaderlabel");

            HidePrompt();
        }
    }

    private void Update()
    {
        // Check if interactable exists
        PerformInteractionCheck();

        if (currentInteractable != null && InputManager.Instance.PlayerActions.Interact.WasPressedThisFrame())
        {
            currentInteractable.Interact();
            UpdatePrompt();
        }
    }

    private void PerformInteractionCheck()
    {
        Ray ray = new(cameraTransform.position, cameraTransform.forward);

        if (Physics.SphereCast(ray, raycastRadius, out RaycastHit raycastHit, raycastDistance))
        {
            IInteractable iinteractable = raycastHit.collider.GetComponent<IInteractable>();

            if (iinteractable is not null)
            {
                if (iinteractable == currentInteractable && iinteractable.GetInteractPrompt() == promptLabel.text)
                    return;

                currentInteractable = iinteractable;
                ShowPrompt();

                return;
            }
        }

        if (currentInteractable != null)
        {
            currentInteractable = null;
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (currentInteractable == null || promptContanier == null || promptHeaderLabel == null || promptLabel == null)
        {
            Debug.LogError("One or more of the visual elements or variable \"currentInteractable\" was null.");
            return;
        }

        UpdatePrompt();
        promptContanier.style.visibility = Visibility.Visible;
    }

    private void HidePrompt() => promptContanier.style.visibility = Visibility.Hidden;

    private void UpdatePrompt()
    {
        promptLabel.text = currentInteractable.GetInteractPrompt();
        promptHeaderLabel.text = $"Press {InputManager.Instance.PlayerActions.Interact.GetBindingDisplayString()} to Interact";
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * raycastDistance);
        Gizmos.DrawWireSphere(cameraTransform.position + (cameraTransform.forward * raycastDistance), raycastRadius);
    }
}