using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

public class PlayerInteraction : MonoBehaviour
{
    // Serialized variables
    [Header("Raycast Configuration")]

    [Description("Reach distance which determines maximum raycast lenght.")]
    [SerializeField] private float raycastDistance = 3.5f;

    [Description("Sphere radius which determines the raycast sphere's radius.")]
    [SerializeField] private float raycastRadius = 0.02f;

    // Injected dependencies
    private IInputService inputService;
    private Transform cameraTransform;
    private UIDocument hudDocument;

    // Private variables
    private IInteractable currentInteractable;
    private RaycastHit currentHit;
    private VisualElement promptContanier;
    private Label promptLabel;
    private Label promptHeaderLabel;

    private bool _initialized = false;

    public void Initialize(IInputService inputService, Transform cameraTransform, UIDocument hudDocument)
    {
        this.inputService = inputService;
        this.cameraTransform = cameraTransform;
        this.hudDocument = hudDocument;

        if (hudDocument == null)
            throw new NullReferenceException("Dependency reference needs to be injected via Bootstrapper.");

        VisualElement root = hudDocument.rootVisualElement;

        promptContanier = root.Q<VisualElement>("PromptLabelContainer");
        promptLabel = root.Q<Label>("PromptLabel");
        promptHeaderLabel = root.Q<Label>("PromptHeaderlabel");

        HidePrompt();
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized)
            return;

        // Check if interactable exists
        PerformInteractionCheck();

        if (currentInteractable != null && inputService.PlayerActions.Interact.WasPressedThisFrame())
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
            currentHit = raycastHit;
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
        promptHeaderLabel.text = $"Press {inputService.PlayerActions.Interact.GetBindingDisplayString()} to Interact";
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * raycastDistance);
        Gizmos.DrawWireSphere(cameraTransform.position + (cameraTransform.forward * currentHit.distance), raycastRadius);
    }
}