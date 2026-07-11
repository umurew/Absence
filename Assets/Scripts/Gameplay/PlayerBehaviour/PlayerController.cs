using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    private PlayerInteraction _playerInteraction;
    private PlayerMovement _playerMovement;

    public void Initialize(IInputService inputService, Transform cameraTransform, UIDocument hudDocument)
    {
        _playerInteraction = GetComponent<PlayerInteraction>();
        _playerMovement = GetComponent<PlayerMovement>();

        if (_playerInteraction != null)
            _playerInteraction.Initialize(inputService, cameraTransform, hudDocument);

        if (_playerMovement != null)
            _playerMovement.Initialize(inputService, cameraTransform);
    }
}